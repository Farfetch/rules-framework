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
    using Rules.Framework.Generic;
    using Rules.Framework.WebUI.Dto;
    using Rules.Framework.WebUI.Handlers;
    using Rules.Framework.WebUI.Tests.Utilities;
    using Xunit;

    public class GetRulesHandlerTests
    {
        private readonly GetRulesHandler handler;
        private readonly Mock<IGenericRulesEngine> rulesEngine;

        public GetRulesHandlerTests()
        {
            this.rulesEngine = new Mock<IGenericRulesEngine>();
            var ruleStatusDtoAnalyzer = new RuleStatusDtoAnalyzer();
            this.handler = new GetRulesHandler(rulesEngine.Object, ruleStatusDtoAnalyzer);
        }

        [Theory]
        [InlineData("POST", "/rules/Rule/List", false, null)]
        [InlineData("GET", "/rules/ContentType/List", false, null)]
        [InlineData("GET", "/rules/Rule/List", true, HttpStatusCode.BadRequest)]
        [InlineData("GET", "/rules/Rule/List", true, HttpStatusCode.OK)]
        [InlineData("GET", "/rules/Rule/List", true, HttpStatusCode.InternalServerError)]
        public async Task HandleRequestAsync_Validation(string httpMethod, string resourcePath,
            bool expectedResult, HttpStatusCode? statusCode)
        {
            //Arrange
            var httpContext = HttpContextHelper.CreateHttpContext(httpMethod, resourcePath);
            var genericRule = new List<GenericRule>();

            if (statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.InternalServerError)
            {
                httpContext.Request.QueryString = new QueryString("?contentType=1");

                if (statusCode == HttpStatusCode.OK)
                {
                    this.rulesEngine.Setup(d => d.SearchAsync(It.IsAny<SearchArgs<GenericContentType, GenericConditionType>>()))
                        .ReturnsAsync(genericRule);
                }

                if (statusCode == HttpStatusCode.InternalServerError)
                {
                    this.rulesEngine.Setup(d => d.SearchAsync(It.IsAny<SearchArgs<GenericContentType, GenericConditionType>>()))
                        .Throws(new Exception("message", new Exception("inner")));
                }
            }
            else
            {
                httpContext.Request.QueryString = new QueryString();
            }
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
                httpContext.Response.StatusCode.Should().Be((int)statusCode);
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