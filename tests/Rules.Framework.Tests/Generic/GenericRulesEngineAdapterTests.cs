namespace Rules.Framework.Tests.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Generic;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class GenericRulesEngineAdapterTests
    {
        private readonly Mock<IRulesEngine<ContentType, ConditionType>> mockRulesEngine;

        public GenericRulesEngineAdapterTests()
        {
            this.mockRulesEngine = new Mock<IRulesEngine<ContentType, ConditionType>>();
        }

        [Fact]
        public void GenericRulesEngineAdapter_GetContentTypes_Success()
        {
            // Arrange
            var expectedGenericContentTypes = new List<GenericContentType>()
            {
                new GenericContentType() { FileName = "Rules.Framework.Tests.TestStubs.ContentType", Name = "Type1" },
                new GenericContentType() { FileName = "Rules.Framework.Tests.TestStubs.ContentType", Name = "Type2" },
            };

            var genericRulesEngineAdapter = new GenericRulesEngineAdapter<ContentType, ConditionType>(this.mockRulesEngine.Object);

            // Act
            var genericContentTypes = genericRulesEngineAdapter.GetContentTypes();

            // Assert
            genericContentTypes.Should().BeEquivalentTo(expectedGenericContentTypes);
        }

        [Fact]
        public void GenericRulesEngineAdapter_GetContentTypes_WithClassContentType_ThrowsException()
        {
            // Arrange
            var mockRulesEngineContentTypeClass = new Mock<IRulesEngine<ContentTypeClass, ConditionType>>();

            var genericRulesEngineAdapter = new GenericRulesEngineAdapter<ContentTypeClass, ConditionType>(mockRulesEngineContentTypeClass.Object);

            // Act and Assert
            Assert.Throws<ArgumentException>(() => genericRulesEngineAdapter.GetContentTypes());
        }

        [Fact]
        public void GenericRulesEngineAdapter_GetContentTypes_WithEmptyContentType_Success()
        {
            // Arrange
            var mockRulesEngineEmptyContentType = new Mock<IRulesEngine<EmptyContentType, ConditionType>>();

            var genericRulesEngineAdapter = new GenericRulesEngineAdapter<EmptyContentType, ConditionType>(mockRulesEngineEmptyContentType.Object);

            // Act
            var genericContentTypes = genericRulesEngineAdapter.GetContentTypes();

            // Assert
            genericContentTypes.Should().BeEmpty();
        }

        [Fact]
        public void GenericRulesEngineAdapter_GetPriorityCriterias_CallsRulesEngineMethod()
        {
            // Arrange
            var expectedPriorityCriteria = PriorityCriterias.TopMostRuleWins;

            this.mockRulesEngine.Setup(m => m.GetPriorityCriterias()).Returns(expectedPriorityCriteria);

            var genericRulesEngineAdapter = new GenericRulesEngineAdapter<ContentType, ConditionType>(this.mockRulesEngine.Object);

            // Act
            var priorityCriteria = genericRulesEngineAdapter.GetPriorityCriterias();

            // Assert
            priorityCriteria.Should().Be(expectedPriorityCriteria);
        }

        [Fact]
        public async Task GenericRulesEngineAdapter_SearchAsync_Success()
        {
            // Arrange
            var expectedGenericRules = new List<GenericRule>()
            {
                new GenericRule()
                {
                    ContentContainer = new ContentContainer<ContentType>(ContentType.Type1, (t) => new object()).GetContentAs<object>(),
                    DateBegin = new DateTime(2018, 01, 01),
                    DateEnd = new DateTime(2019, 01, 01),
                    Name = "Test rule",
                    Priority = 3,
                    RootCondition = new GenericValueConditionNode()
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
            var genericContentType = new GenericContentType { Name = "Type1", FileName = "Rules.Framework.Tests.TestStubs.ContentType" };

            var genericSearchArgs = new SearchArgs<GenericContentType, GenericConditionType>(genericContentType, dateBegin, dateEnd);

            var testRules = new List<Rule<ContentType, ConditionType>>()
            {
                new Rule<ContentType, ConditionType>
                {
                    ContentContainer = new ContentContainer<ContentType>(ContentType.Type1, (t) => new object()),
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

            var genericRulesEngineAdapter = new GenericRulesEngineAdapter<ContentType, ConditionType>(this.mockRulesEngine.Object);

            // Act
            var genericRules = await genericRulesEngineAdapter.SearchAsync(genericSearchArgs);

            // Assert
            genericRules.Should().BeEquivalentTo(expectedGenericRules);
        }
    }
}