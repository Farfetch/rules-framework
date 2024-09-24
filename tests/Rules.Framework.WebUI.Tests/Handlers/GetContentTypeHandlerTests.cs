namespace Rules.Framework.WebUI.Tests.Handlers
{
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using Rules.Framework.WebUI.Dto;
    using Rules.Framework.WebUI.Handlers;
    using Rules.Framework.WebUI.Tests.Utilities;
    using Xunit;

    public class GetContentTypeHandlerTests
    {
        private readonly GetContentTypeHandler handler;
        private readonly Mock<IRulesEngine> rulesEngine;

        public GetContentTypeHandlerTests()
        {
            var ruleStatusDtoAnalyzer = new RuleStatusDtoAnalyzer();
            this.rulesEngine = new Mock<IRulesEngine>();
            this.handler = new GetContentTypeHandler(rulesEngine.Object, ruleStatusDtoAnalyzer, new WebUIOptions());
        }

        [Theory]
        [InlineData("POST", "/rules/api/v1/contentTypes", false)]
        [InlineData("GET", "/rules/api/v1/rules", false)]
        [InlineData("GET", "/rules/api/v1/contentTypes", true)]
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
                this.rulesEngine.Verify(s => s.GetContentTypesAsync(), Times.Once);
            }
            else
            {
                this.rulesEngine.Verify(s => s.GetContentTypesAsync(), Times.Never);
            }
        }
    }
}