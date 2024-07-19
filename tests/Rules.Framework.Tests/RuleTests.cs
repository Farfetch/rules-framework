namespace Rules.Framework.Tests
{
    using System;
    using FluentAssertions;
    using Moq;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RuleTests
    {
        [Fact]
        public void Clone_WithRuleWithoutRootCondition_ReturnsCopy()
        {
            // Arrange
            var rule = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = DateTime.UtcNow.AddDays(-1),
                DateEnd = DateTime.UtcNow.AddDays(1),
                Priority = 1,
                Name = "Name",
                RootCondition = null,
            };

            // Act
            var actual = rule.Clone();

            // Assert
            actual.Should().BeEquivalentTo(rule);
        }

        [Fact]
        public void Clone_WithRuleWithRootCondition_ReturnsCopy()
        {
            // Arrange
            var rule = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = DateTime.UtcNow.AddDays(-1),
                DateEnd = DateTime.UtcNow.AddDays(1),
                Priority = 1,
                Name = "Name",
                RootCondition = new ValueConditionNode(DataTypes.Decimal, ConditionNames.PluviosityRate.ToString(), Operators.GreaterThanOrEqual, 80.0d),
            };
            rule.RootCondition.Properties["key1"] = "value1";
            rule.RootCondition.Properties["key2"] = new object();

            // Act
            var actual = rule.Clone();

            // Assert
            actual.Should().BeEquivalentTo(rule);
        }

        [Fact]
        public void ContentContainer_HavingSettedInstance_ReturnsProvidedInstance()
        {
            // Arrange
            var expected = new ContentContainer((_) => null);

            var sut = new Rule
            {
                ContentContainer = expected
            };

            // Act
            var actual = sut.ContentContainer;

            // Assert
            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void DateBegin_HavingSettedValue_ReturnsProvidedValue()
        {
            // Arrange
            var expected = new DateTime(2018, 07, 19);

            var sut = new Rule
            {
                DateBegin = expected
            };

            // Act
            var actual = sut.DateBegin;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void DateEnd_HavingSettedValue_ReturnsProvidedValue()
        {
            // Arrange
            var expected = new DateTime(2018, 07, 19);

            var sut = new Rule
            {
                DateEnd = expected
            };

            // Act
            var actual = sut.DateEnd;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void DateEnd_NotHavingSettedValue_ReturnsNull()
        {
            // Arrange
            var sut = new Rule();

            // Act
            var actual = sut.DateEnd;

            // Assert
            actual.Should().BeNull();
        }

        [Fact]
        public void Name_HavingSettedValue_ReturnsProvidedValue()
        {
            // Arrange
            var expected = "My awesome name";

            var sut = new Rule
            {
                Name = expected
            };

            // Act
            var actual = sut.Name;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Priority_HavingSettedValue_ReturnsProvidedValue()
        {
            // Arrange
            var expected = 123;

            var sut = new Rule
            {
                Priority = expected
            };

            // Act
            var actual = sut.Priority;

            // Arrange
            actual.Should().Be(expected);
        }

        [Fact]
        public void RootCondition_HavingSettedInstance_ReturnsProvidedInstance()
        {
            // Arrange
            var mockConditionNode = new Mock<IConditionNode>();
            var expected = mockConditionNode.Object;

            var sut = new Rule
            {
                RootCondition = expected
            };

            // Act
            var actual = sut.RootCondition;

            // Assert
            actual.Should().BeSameAs(expected);
        }
    }
}