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
    using Rules.Framework.Generics;
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
            this.handler = new GetRulesHandler(rulesEngine.Object, ruleStatusDtoAnalyzer, new WebUIOptions());
        }

        [Theory]
        [InlineData("GET", "/rules/api/v1/rules", HttpStatusCode.OK)]
        [InlineData("GET", "/rules/api/v1/rules", HttpStatusCode.InternalServerError)]
        public async Task HandleRequestAsync_Validation(string httpMethod, string resourcePath,
             HttpStatusCode? statusCode)
        {
            //Arrange
            var httpContext = HttpContextHelper.CreateHttpContext(httpMethod, resourcePath);
            var genericRule = new List<GenericRule>();
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

            //Act
            await this.handler.HandleAsync(httpContext).ConfigureAwait(false);

            //Assert
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

            this.rulesEngine
                .Verify(s => s.SearchAsync(It.IsAny<SearchArgs<GenericContentType, GenericConditionType>>()), Times.Once);
        }
    }
}