namespace Rules.Framework.WebUI.Tests.Handlers
{
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Generics;
    using Rules.Framework.WebUI.Dto;
    using Rules.Framework.WebUI.Handlers;
    using Rules.Framework.WebUI.Tests.Utilities;
    using Xunit;

    public class GetContentTypeHandlerTests
    {
        private readonly GetContentTypeHandler handler;
        private readonly Mock<IGenericRulesEngine> rulesEngine;

        public GetContentTypeHandlerTests()
        {
            var ruleStatusDtoAnalyzer = new RuleStatusDtoAnalyzer();
            this.rulesEngine = new Mock<IGenericRulesEngine>();
            this.handler = new GetContentTypeHandler(rulesEngine.Object, ruleStatusDtoAnalyzer, new WebUIOptions());
        }

        [Fact]
        public async Task HandleRequestAsync_Validation()
        {
            //Arrange
            var httpContext = HttpContextHelper.CreateHttpContext("GET", "/rules/api/v1/contentTypes");
            //Act
            await this.handler.HandleAsync(httpContext).ConfigureAwait(false);

            //Assert
            httpContext.Response.Should().NotBeNull();
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            httpContext.Response.ContentType.Should().Be("application/json");
            string body = string.Empty;
            using (var reader = new StreamReader(httpContext.Response.Body))
            {
                httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                body = await reader.ReadToEndAsync();
            }
            body.Should().NotBeNullOrWhiteSpace();
            httpContext.Response.ContentLength.Should().Be(body.Length);
            this.rulesEngine.Verify(s => s.GetContentTypes(), Times.Once);
        }
    }
}