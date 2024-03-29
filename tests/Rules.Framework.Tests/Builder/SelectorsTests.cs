namespace Rules.Framework.Tests.Builder
{
    using System;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Builder;
    using Rules.Framework.Tests.Stubs;
    using Xunit;
    using static Rules.Framework.Builder.RulesEngineSelectors;

    public class SelectorsTests
    {
        [Fact]
        public void ConditionTypeSelector_WithConditionType_GivenTypeOfCondition_ReturnsNewRulesDataSourceSelector()
        {
            // Arrange
            ConditionTypeSelector<ContentType> sut = new ConditionTypeSelector<ContentType>();

            // Act
            IRulesDataSourceSelector<ContentType, ConditionType> actual = sut.WithConditionType<ConditionType>();

            // Assert
            actual.Should().BeOfType<RulesDataSourceSelector<ContentType, ConditionType>>();
        }

        [Fact]
        public void ContentTypeSelector_WithContentType_GivenTypeOfContent_ReturnsNewConditionTypeSelector()
        {
            // Arrange
            ContentTypeSelector sut = new ContentTypeSelector();

            // Act
            IConditionTypeSelector<ContentType> actual = sut.WithContentType<ContentType>();

            // Assert
            actual.Should().BeOfType<ConditionTypeSelector<ContentType>>();
        }

        [Fact]
        public void RulesDataSourceSelector_SetDataSource_GivenNullRulesDataSource_ThrowsArgumentNullException()
        {
            // Arrange
            RulesDataSourceSelector<ContentType, ConditionType> sut = new RulesDataSourceSelector<ContentType, ConditionType>();

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
            RulesDataSourceSelector<ContentType, ConditionType> sut = new RulesDataSourceSelector<ContentType, ConditionType>();

            Mock<IRulesDataSource<ContentType, ConditionType>> mockRulesDataSource = new Mock<IRulesDataSource<ContentType, ConditionType>>();

            // Act
            IConfiguredRulesEngineBuilder<ContentType, ConditionType> actual = sut.SetDataSource(mockRulesDataSource.Object);

            // Assert
            actual.Should().NotBeNull();
        }
    }
}