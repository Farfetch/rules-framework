namespace Rules.Framework.WebUI.Tests.Handlers
{
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using Rules.Framework.WebUI.Handlers;
    using Rules.Framework.WebUI.Tests.Utilities;
    using Xunit;

    public class GetConfigurationsHandlerTests
    {
        private readonly GetConfigurationsHandler handler;
        private readonly Mock<IRulesEngine> rulesEngine;

        public GetConfigurationsHandlerTests()
        {
            this.rulesEngine = new Mock<IRulesEngine>();
            this.rulesEngine
                .SetupGet(x => x.Options)
                .Returns(RulesEngineOptions.NewWithDefaults());
            this.handler = new GetConfigurationsHandler(rulesEngine.Object, new WebUIOptions());
        }

        [Theory]
        [InlineData("POST", "/rules/api/v1/configurations", false)]
        [InlineData("GET", "/rules/api/v1/rulesets", false)]
        [InlineData("GET", "/rules/api/v1/configurations", true)]
        public async Task HandleRequestAsync_Validation(string httpMethod, string resourcePath,
            bool expectedResult)
        {
            //Arrange
            var httpContext = HttpContextHelper.CreateHttpContext(httpMethod, resourcePath);
            RequestDelegate next = (HttpContext _) => Task.CompletedTask;

            //Act
            var result = await this.handler.HandleAsync(httpContext.Request, httpContext.Response, next);

            //Assert
            result.Should().Be(expectedResult);
            if (expectedResult)
            {
                httpContext.Response.Should().NotBeNull();
                httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
                httpContext.Response.ContentType.Should().Be("application/json");
                var body = string.Empty;
                using (var reader = new StreamReader(httpContext.Response.Body))
                {
                    httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                    body = await reader.ReadToEndAsync();
                }
                body.Should().NotBeNullOrWhiteSpace();
                httpContext.Response.ContentLength.Should().Be(body.Length);
                this.rulesEngine.Verify(s => s.Options, Times.AtLeastOnce());
            }
            else
            {
                this.rulesEngine.Verify(s => s.Options, Times.Never());
            }
        }
    }
}