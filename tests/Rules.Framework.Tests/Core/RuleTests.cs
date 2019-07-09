namespace Rules.Framework.Tests.Core
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Tests.TestStubs;

    [TestClass]
    public class RuleTests
    {
        [TestMethod]
        public void Rule_ContentContainer_ContentContainer_HavingSettedInstance_ReturnsProvidedInstance()
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
            Assert.AreSame(expected, actual);
        }

        [TestMethod]
        public void Rule_DateBegin_HavingSettedValue_ReturnsProvidedValue()
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
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Rule_DateEnd_HavingSettedValue_ReturnsProvidedValue()
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
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Rule_DateEnd_NotHavingSettedValue_ReturnsNull()
        {
            // Arrange
            Rule<ContentType, ConditionType> sut = new Rule<ContentType, ConditionType>();

            // Act
            DateTime? actual = sut.DateEnd;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void Rule_Name_HavingSettedValue_ReturnsProvidedValue()
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
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Rule_Priority_HavingSettedValue_ReturnsProvidedValue()
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
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Rule_RootCondition_HavingSettedInstance_ReturnsProvidedInstance()
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
            Assert.AreSame(expected, actual);
        }
    }
}