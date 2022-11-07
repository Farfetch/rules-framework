namespace Rules.Framework.WebUI.Tests.Handlers
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Rules.Framework.WebUI.Handlers;
    using Rules.Framework.WebUI.Tests.Utilities;
    using Xunit;

    public class GetIndexPageHandlerTests
    {
        private readonly GetIndexPageHandler handler;

        public GetIndexPageHandlerTests()
        {
            this.handler = new GetIndexPageHandler(new WebUIOptions());
        }

        [Theory]
        [InlineData("POST", "/rules/index.html", false)]
        [InlineData("GET", "/rules/Rule/List", false)]
        [InlineData("GET", "/rules/index.html", true)]
        [InlineData("GET", "/rules", true)]
        public async Task HandleRequestAsync_Validation(string httpMethod, string resourcePath,
            bool expectedResult)
        {
            //Arrange
            var httpContext = HttpContextHelper.CreateHttpContext(httpMethod, resourcePath);

            //Act
            var result = await this.handler
                .HandleAsync(httpContext.Request, httpContext.Response)
                .ConfigureAwait(false);

            //Assert
            result.Should().Be(expectedResult);
        }
    }
}