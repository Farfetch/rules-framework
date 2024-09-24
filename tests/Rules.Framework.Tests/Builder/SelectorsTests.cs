namespace Rules.Framework.Tests.Builder
{
    using System;
    using FluentAssertions;
    using Moq;
    using Xunit;
    using static Rules.Framework.Builder.RulesEngineSelectors;

    public class SelectorsTests
    {
        [Fact]
        public void RulesDataSourceSelector_SetDataSource_GivenNullRulesDataSource_ThrowsArgumentNullException()
        {
            // Arrange
            var sut = new RulesDataSourceSelector();

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                sut.SetDataSource(null);
            });
        }

        [Fact]
        public void RulesDataSourceSelector_SetDataSource_GivenRulesDataSourceInstance_ReturnsRulesEngine()
        {
            // Arrange
            var sut = new RulesDataSourceSelector();

            var mockRulesDataSource = new Mock<IRulesDataSource>();

            // Act
            var actual = sut.SetDataSource(mockRulesDataSource.Object);

            // Assert
            actual.Should().NotBeNull();
        }
    }
}