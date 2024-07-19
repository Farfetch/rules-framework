namespace Rules.Framework.WebUI.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
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

    public class GetRulesHandlerTests
    {
        private readonly GetRulesHandler handler;
        private readonly Mock<IRulesEngine> rulesEngine;

        public GetRulesHandlerTests()
        {
            this.rulesEngine = new Mock<IRulesEngine>();
            this.rulesEngine.SetupGet(x => x.Options)
                .Returns(RulesEngineOptions.NewWithDefaults());
            var ruleStatusDtoAnalyzer = new RuleStatusDtoAnalyzer();
            this.handler = new GetRulesHandler(rulesEngine.Object, ruleStatusDtoAnalyzer, new WebUIOptions());
        }

        [Theory]
        [InlineData("POST", "/rules/api/v1/rules", false, null)]
        [InlineData("GET", "/rules/api/v1/contentTypes", false, null)]
        [InlineData("GET", "/rules/api/v1/rules", true, HttpStatusCode.OK)]
        [InlineData("GET", "/rules/api/v1/rules", true, HttpStatusCode.InternalServerError)]
        public async Task HandleRequestAsync_Validation(string httpMethod, string resourcePath,
            bool expectedResult, HttpStatusCode? statusCode)
        {
            //Arrange
            var httpContext = HttpContextHelper.CreateHttpContext(httpMethod, resourcePath);
            var genericRule = new List<Rule>();
            var verifySearchAsync = false;

            if (statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.InternalServerError)
            {
                verifySearchAsync = true;

                httpContext.Request.QueryString = new QueryString("?contentType=1");

                if (statusCode == HttpStatusCode.OK)
                {
                    this.rulesEngine.Setup(d => d.SearchAsync(It.IsAny<SearchArgs<string, string>>()))
                        .ReturnsAsync(genericRule);
                }

                if (statusCode == HttpStatusCode.InternalServerError)
                {
                    this.rulesEngine.Setup(d => d.SearchAsync(It.IsAny<SearchArgs<string, string>>()))
                        .Throws(new Exception("message", new Exception("inner")));
                }
            }
            else
            {
                httpContext.Request.QueryString = new QueryString();
            }
            RequestDelegate next = (HttpContext _) => Task.CompletedTask;

            //Act
            var result = await this.handler.HandleAsync(httpContext.Request, httpContext.Response, next);

            //Assert
            result.Should().Be(expectedResult);
            if (expectedResult)
            {
                httpContext.Response.Should().NotBeNull();
                httpContext.Response.StatusCode.Should().Be((int)statusCode);
                httpContext.Response.ContentType.Should().Be("application/json");
                var body = string.Empty;
                using (var reader = new StreamReader(httpContext.Response.Body))
                {
                    httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                    body = await reader.ReadToEndAsync();
                }
                body.Should().NotBeNullOrWhiteSpace();
                httpContext.Response.ContentLength.Should().Be(body.Length);
            }

            if (verifySearchAsync)
            {
                this.rulesEngine
                    .Verify(s => s.SearchAsync(It.IsAny<SearchArgs<string, string>>()), Times.Once);
            }
            else
            {
                this.rulesEngine
                    .Verify(s => s.SearchAsync(It.IsAny<SearchArgs<string, string>>()), Times.Never);
            }
        }
    }
}