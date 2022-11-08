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

            mockLoggerFactory.Setup(d => d.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
            var middleware = WebUIMiddlewareFactory.Create(mockWebHostEnvironment.Object,
                mockLoggerFactory.Object,
                mockHttpRequestHandler.Object);

            var context = new DefaultHttpContext();
            RequestDelegate next = (HttpContext hc) => Task.CompletedTask;

            // act
            await middleware.InvokeAsync(context);

            // assert
            mockHttpRequestHandler.Verify(mock => mock.HandleAsync(It.IsAny<HttpRequest>(), It.IsAny<HttpResponse>(), It.IsAny<RequestDelegate>()), Times.Once());
        }
    }
}