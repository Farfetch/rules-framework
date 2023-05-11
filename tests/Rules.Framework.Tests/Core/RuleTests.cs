namespace Rules.Framework.Tests.Core
{
    using System;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RuleTests
    {
        [Fact]
        public void Clone_WithRuleWithoutRootCondition_ReturnsCopy()
        {
            // Arrange
            var rule = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(ContentType.Type1, _ => new object()),
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
            var rule = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(ContentType.Type1, _ => new object()),
                DateBegin = DateTime.UtcNow.AddDays(-1),
                DateEnd = DateTime.UtcNow.AddDays(1),
                Priority = 1,
                Name = "Name",
                RootCondition = new ValueConditionNode<ConditionType>(DataTypes.Decimal, ConditionType.PluviosityRate, Operators.GreaterThanOrEqual, 80.0d),
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
            var expected = new ContentContainer<ContentType>(ContentType.Type1, (t) => null);

            var sut = new Rule<ContentType, ConditionType>
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

            var sut = new Rule<ContentType, ConditionType>
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

            var sut = new Rule<ContentType, ConditionType>
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
            var sut = new Rule<ContentType, ConditionType>();

            // Act
            var actual = sut.DateEnd;

            // Assert
            actual.Should().BeNull();
        }

        [Fact]
        public void Name_HavingSettedValue_ReturnsProvidedValue()
        {
            // Arrange
            string expected = "My awesome name";

            var sut = new Rule<ContentType, ConditionType>
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

            var sut = new Rule<ContentType, ConditionType>
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
            var mockConditionNode = new Mock<IConditionNode<ConditionType>>();
            var expected = mockConditionNode.Object;

            var sut = new Rule<ContentType, ConditionType>
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