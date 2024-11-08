namespace Rules.Framework.WebUI.Tests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class WebUIOptionsRegistryTests
    {
        [Fact]
        public void Register_GivenNullOptions_ThrowsArgumentNullException()
        {
            // Arrange
            var webUIOptionsRegistry = new WebUIOptionsRegistry();

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => webUIOptionsRegistry.Register(null));

            // Assert
            exception.ParamName.Should().Be("webUIOptions");
            webUIOptionsRegistry.RegisteredOptions.Should().BeNull();
        }

        [Fact]
        public void Register_GivenValidOptions_RegistersOptions()
        {
            // Arrange
            var webUIOptions = new WebUIOptions
            {
                DocumentTitle = "Title",
            };
            var webUIOptionsRegistry = new WebUIOptionsRegistry();

            // Act
            webUIOptionsRegistry.Register(webUIOptions);

            // Assert
            webUIOptionsRegistry.RegisteredOptions.Should().NotBeNull()
                .And.BeSameAs(webUIOptions);
        }
    }
}