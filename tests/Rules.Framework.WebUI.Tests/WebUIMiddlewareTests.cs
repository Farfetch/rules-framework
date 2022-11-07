namespace Rules.Framework.WebUI.Tests
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Rules.Framework.WebUI.Tests.Utilities;
    using Xunit;
    using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

    public class WebUIMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_CallsHttpRequestHandler()
        {
            var mockHttpRequestHandler = new Mock<IHttpRequestHandler>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger>();
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

            //Microsoft.Extensions.Logging.Logger`1.Microsoft.Extensions.Logging.ILogger.IsEnabled(LogLevel logLevel)
            mockLoggerFactory.Setup(d => d.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var middleware = WebUIMiddlewareFactory.Create(mockWebHostEnvironment.Object,
                mockLoggerFactory.Object,
                mockHttpRequestHandler.Object);

            var context = new DefaultHttpContext();

            // act
            await middleware.InvokeAsync(context);

            // assert
            mockHttpRequestHandler.Verify(mock => mock.HandleAsync(context.Request, context.Response), Times.Once());
        }
    }
}