namespace Rules.Framework.Tests.Generics
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;    
    using Rules.Framework.Generics;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class GenericRulesEngineTests
    {
        private readonly Mock<IRulesEngine<ContentType, ConditionType>> mockRulesEngine;

        public GenericRulesEngineTests()
        {
            this.mockRulesEngine = new Mock<IRulesEngine<ContentType, ConditionType>>();
        }

        [Fact]
        public void GenericRulesEngine_GetContentTypes_Success()
        {
            // Arrange
            var expectedGenericContentTypes = new List<GenericContentType>
            {
                new GenericContentType { Identifier = "Type1" },
                new GenericContentType { Identifier = "Type2" },
            };

            var genericRulesEngine = new GenericRulesEngine<ContentType, ConditionType>(this.mockRulesEngine.Object);

            // Act
            var genericContentTypes = genericRulesEngine.GetContentTypes();

            // Assert
            genericContentTypes.Should().BeEquivalentTo(expectedGenericContentTypes);
        }

        [Fact]
        public void GenericRulesEngine_GetContentTypes_WithClassContentType_ThrowsException()
        {
            // Arrange
            var mockRulesEngineContentTypeClass = new Mock<IRulesEngine<ContentTypeClass, ConditionType>>();

            var genericRulesEngine = new GenericRulesEngine<ContentTypeClass, ConditionType>(mockRulesEngineContentTypeClass.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => genericRulesEngine.GetContentTypes());
        }

        [Fact]
        public void GenericRulesEngine_GetContentTypes_WithEmptyContentType_Success()
        {
            // Arrange
            var mockRulesEngineEmptyContentType = new Mock<IRulesEngine<EmptyContentType, ConditionType>>();

            var genericRulesEngine = new GenericRulesEngine<EmptyContentType, ConditionType>(mockRulesEngineEmptyContentType.Object);

            // Act
            var genericContentTypes = genericRulesEngine.GetContentTypes();

            // Assert
            genericContentTypes.Should().BeEmpty();
        }

        [Fact]
        public void GenericRulesEngine_GetPriorityCriterias_CallsRulesEngineMethod()
        {
            // Arrange
            var expectedPriorityCriteria = PriorityCriterias.TopmostRuleWins;

            this.mockRulesEngine.Setup(m => m.GetPriorityCriteria()).Returns(expectedPriorityCriteria);

            var genericRulesEngine = new GenericRulesEngine<ContentType, ConditionType>(this.mockRulesEngine.Object);

            // Act
            var priorityCriteria = genericRulesEngine.GetPriorityCriteria();

            // Assert
            priorityCriteria.Should().Be(expectedPriorityCriteria);
            this.mockRulesEngine.Verify(m => m.GetPriorityCriteria(), Times.Once);
        }

        [Fact]
        public async Task GenericRulesEngine_SearchAsync_Success()
        {
            // Arrange
            var expectedGenericRules = new List<GenericRule>
            {
                new GenericRule
                {
                    ContentContainer = new ContentContainer<ContentType>(ContentType.Type1, (_) => new object()).GetContentAs<object>(),
                    DateBegin = new DateTime(2018, 01, 01),
                    DateEnd = new DateTime(2019, 01, 01),
                    Name = "Test rule",
                    Priority = 3,
                    RootCondition = new GenericValueConditionNode
                    {
                        ConditionTypeName =  ConditionType.IsoCountryCode.ToString(),
                        DataType = DataTypes.String,
                        Operator = Operators.Equal,
                        Operand = "USA"
                    }
                }
            };

            var dateBegin = new DateTime(2022, 01, 01);
            var dateEnd = new DateTime(2022, 12, 01);
            var genericContentType = new GenericContentType { Identifier = "Type1" };

            var genericSearchArgs = new SearchArgs<GenericContentType, GenericConditionType>(genericContentType, dateBegin, dateEnd);

            var testRules = new List<Rule<ContentType, ConditionType>>
            {
                new Rule<ContentType, ConditionType>
                {
                    ContentContainer = new ContentContainer<ContentType>(ContentType.Type1, (_) => new object()),
                    DateBegin = new DateTime(2018, 01, 01),
                    DateEnd = new DateTime(2019, 01, 01),
                    Name = "Test rule",
                    Priority = 3,
                    RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "USA")
                }
            };

            this.mockRulesEngine
                .Setup(m => m.SearchAsync(It.IsAny<SearchArgs<ContentType, ConditionType>>()))
                .ReturnsAsync(testRules);

            var genericRulesEngine = new GenericRulesEngine<ContentType, ConditionType>(this.mockRulesEngine.Object);

            // Act
            var genericRules = await genericRulesEngine.SearchAsync(genericSearchArgs);

            // Assert
            genericRules.Should().BeEquivalentTo(expectedGenericRules);
            this.mockRulesEngine
                .Verify(m => m.SearchAsync(It.IsAny<SearchArgs<ContentType, ConditionType>>()), Times.Once);
        }
    }
}
