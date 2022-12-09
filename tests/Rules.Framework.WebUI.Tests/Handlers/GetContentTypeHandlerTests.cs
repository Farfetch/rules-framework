namespace Rules.Framework.WebUI.Tests.Handlers
{
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using Rules.Framework.Generics;
    using Rules.Framework.WebUI.Dto;
    using Rules.Framework.WebUI.Handlers;
    using Rules.Framework.WebUI.Tests.Utilities;
    using Xunit;

    public class GetContentTypeHandlerTests
    {
        private readonly GetContentTypeHandler handler;

        public GetContentTypeHandlerTests()
        {
            var ruleStatusDtoAnalyzer = new RuleStatusDtoAnalyzer();
            var rulesEngine = new Mock<IGenericRulesEngine>();
            this.handler = new GetContentTypeHandler(rulesEngine.Object, ruleStatusDtoAnalyzer);
        }

        [Theory]
        [InlineData("POST", "/rules/ContentType/List", false)]
        [InlineData("GET", "/rules/Rule/List", false)]
        [InlineData("GET", "/rules/ContentType/List", true)]
        public async Task HandleRequestAsync_Validation(string httpMethod, string resourcePath,
            bool expectedResult)
        {
            //Arrange
            var httpContext = HttpContextHelper.CreateHttpContext(httpMethod, resourcePath);
            RequestDelegate next = (HttpContext _) => Task.CompletedTask;
            //Act
            var result = await this.handler
                .HandleAsync(httpContext.Request, httpContext.Response, next)
                .ConfigureAwait(false);

            //Assert
            result.Should().Be(expectedResult);
            if (expectedResult)
            {
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
            }
        }
    }
}