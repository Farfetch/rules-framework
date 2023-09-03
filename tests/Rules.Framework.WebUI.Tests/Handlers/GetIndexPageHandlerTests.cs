namespace Rules.Framework.WebUI.Tests.Handlers
{
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using FluentAssertions;
    using RazorLight;
    using Rules.Framework.WebUI.Handlers;
    using Rules.Framework.WebUI.Tests.Utilities;
    using Xunit;

    public class GetIndexPageHandlerTests
    {
        private readonly GetIndexPageHandler handler;

        public GetIndexPageHandlerTests()
        {
            var razorLightEngine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(typeof(WebUIOptions))
                .SetOperatingAssembly(typeof(WebUIOptions).Assembly)
                .UseMemoryCachingProvider()
                .Build();
            this.handler = new GetIndexPageHandler(razorLightEngine, new WebUIOptions());
        }

        [Theory]
        [InlineData("GET", "/rules/index.html", HttpStatusCode.OK)]
        [InlineData("GET", "/rules", HttpStatusCode.MovedPermanently)]
        public async Task HandleRequestAsync_Validation(string httpMethod, string resourcePath, HttpStatusCode httpStatusCode)
        {
            //Arrange
            var httpContext = HttpContextHelper.CreateHttpContext(httpMethod, resourcePath);

            //Act
            await this.handler.HandleAsync(httpContext).ConfigureAwait(false);

            //Assert
            httpContext.Response.Should().NotBeNull();
            httpContext.Response.StatusCode.Should().Be((int)httpStatusCode);
            if (httpStatusCode == HttpStatusCode.OK)
            {
                httpContext.Response.ContentType.Should().Be("text/html;charset=utf-8");
                string body = string.Empty;
                using (var reader = new StreamReader(httpContext.Response.Body))
                {
                    httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                    body = await reader.ReadToEndAsync();
                }
                body.Should().NotBeNullOrWhiteSpace();
            }
            else
            {
                httpContext.Response.Headers.Should().Contain("Location", new[] { "rules/index.html" });
            }
        }
    }
}