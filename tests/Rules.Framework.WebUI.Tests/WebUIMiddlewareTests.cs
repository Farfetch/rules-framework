namespace Rules.Framework.WebUI.Tests
{
    using System.Net;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Rules.Framework.WebUI.Tests.Utilities;
    using Xunit;

    public class WebUIMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_CallsHttpRequestHandler()
        {
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

            mockLoggerFactory.Setup(d => d.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var middleware = WebUIMiddlewareFactory.Create(mockWebHostEnvironment.Object,
                mockLoggerFactory.Object);

            var context = HttpContextHelper.CreateHttpContext("GET", "/node_modules/rules_list.ico");

            // act
            await middleware.InvokeAsync(context);

            // assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
    }
}