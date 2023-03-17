namespace Rules.Framework.Tests.Core
{
    using System;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RuleTests
    {
        [Fact]
        public void ContentContainer_ContentContainer_HavingSettedInstance_ReturnsProvidedInstance()
        {
            // Arrange
            ContentContainer<ContentType> expected = new ContentContainer<ContentType>(ContentType.Type1, (t) => null);

            Rule<ContentType, ConditionType> sut = new Rule<ContentType, ConditionType>
            {
                ContentContainer = expected
            };

            // Act
            ContentContainer<ContentType> actual = sut.ContentContainer;

            // Assert
            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void DateBegin_HavingSettedValue_ReturnsProvidedValue()
        {
            // Arrange
            DateTime expected = new DateTime(2018, 07, 19);

            Rule<ContentType, ConditionType> sut = new Rule<ContentType, ConditionType>
            {
                DateBegin = expected
            };

            // Act
            DateTime actual = sut.DateBegin;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void DateEnd_HavingSettedValue_ReturnsProvidedValue()
        {
            // Arrange
            DateTime expected = new DateTime(2018, 07, 19);

            Rule<ContentType, ConditionType> sut = new Rule<ContentType, ConditionType>
            {
                DateEnd = expected
            };

            // Act
            DateTime? actual = sut.DateEnd;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void DateEnd_NotHavingSettedValue_ReturnsNull()
        {
            // Arrange
            Rule<ContentType, ConditionType> sut = new Rule<ContentType, ConditionType>();

            // Act
            DateTime? actual = sut.DateEnd;

            // Assert
            actual.Should().BeNull();
        }

        [Fact]
        public void Name_HavingSettedValue_ReturnsProvidedValue()
        {
            // Arrange
            string expected = "My awesome name";

            Rule<ContentType, ConditionType> sut = new Rule<ContentType, ConditionType>
            {
                Name = expected
            };

            // Act
            string actual = sut.Name;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Priority_HavingSettedValue_ReturnsProvidedValue()
        {
            // Arrange
            int expected = 123;

            Rule<ContentType, ConditionType> sut = new Rule<ContentType, ConditionType>
            {
                Priority = expected
            };

            // Act
            int actual = sut.Priority;

            // Arrange
            actual.Should().Be(expected);
        }

        [Fact]
        public void RootCondition_HavingSettedInstance_ReturnsProvidedInstance()
        {
            // Arrange
            Mock<IConditionNode<ConditionType>> mockConditionNode = new Mock<IConditionNode<ConditionType>>();
            IConditionNode<ConditionType> expected = mockConditionNode.Object;

            Rule<ContentType, ConditionType> sut = new Rule<ContentType, ConditionType>
            {
                RootCondition = expected
            };

            // Act
            IConditionNode<ConditionType> actual = sut.RootCondition;

            // Assert
            actual.Should().BeSameAs(expected);
        }
    }
}