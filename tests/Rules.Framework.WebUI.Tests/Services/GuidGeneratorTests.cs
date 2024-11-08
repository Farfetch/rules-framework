namespace Rules.Framework.WebUI.Tests.Services
{
    using System;
    using FluentAssertions;
    using Rules.Framework.WebUI.Services;
    using Xunit;

    public class GuidGeneratorTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void GenerateFromString_GivenEmptyOrWhiteSpaceSourceString_ThrowsException(string source)
        {
            // Act
            var actual = Assert.Throws<ArgumentException>(() => GuidGenerator.GenerateFromString(source));

            // Assert
            actual.Should().BeOfType<ArgumentException>();
            actual.ParamName.Should().Be("source");
        }

        [Fact]
        public void GenerateFromString_GivenNullSourceString_ThrowsException()
        {
            // Act
            var actual = Assert.Throws<ArgumentNullException>(() => GuidGenerator.GenerateFromString(null));

            // Assert
            actual.Should().BeOfType<ArgumentNullException>();
            actual.ParamName.Should().Be("source");
        }

        [Theory]
        [InlineData("Test string", "849de4a3-f13d-2e3c-2a77-86f6ecd7e0d1")]
        [InlineData("Yet another test string", "1ea74e0c-ac94-db72-ad40-e19920a0fa00")]
        public void GenerateFromString_GivenSourceString_ReturnsGuid(string source, string expectedGuid)
        {
            // Arrange
            var expected = Guid.Parse(expectedGuid);

            // Act
            var actual = GuidGenerator.GenerateFromString(source);

            // Assert
            actual.Should().Be(expected);
        }
    }
}