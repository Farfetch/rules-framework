namespace Rules.Framework.Providers.SqlServer.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Providers.SqlServer.Tests.Helpers;
    using Rules.Framework.Serialization;
    using Rules.Framework.SqlServer.Models;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class RuleFactoryTests
    {
        [Fact]
        public void RuleFactory_CreateRule_SqlDataModel_Success()
        {
            // Arrange
            var rule = new Rule()
            {
                ConditionNode = new ConditionNode()
                {
                    LogicalOperator = new LogicalOperator()
                    {
                        Name = "Eval",
                        Code = 3,
                        Symbol = "<",
                        ConditionNodes = new List<ConditionNode>()
                    },
                    LogicalOperatorCode = 3,
                    DataTypeCode = 1,
                    Id = 1,
                    Operand = "1",
                    Operator = new Operator()
                    {
                        Code = 1,
                        Name = "Test"
                    },
                    OperatorCode = 1,
                    ConditionType = new ConditionType() { Code = 1, Name = "Test" },
                    ConditionTypeCode = 1
                },
                ConditionNodeId = 1,
                Content = "Test",
                ContentType = new ContentType(),
                ContentTypeCode = 1,
                DateBegin = new DateTime(2023, 1, 1),
                DateEnd = new DateTime(2023, 2, 1),
                Name = "CarInsuranceAdvice",
                Priority = 1
            };

            var contentSerializationProvider = new Mock<IContentSerializationProvider<MockContentType>>();

            var ruleFactory = new RuleFactory<MockContentType, MockConditionType>(contentSerializationProvider.Object);

            // Act
            var result = ruleFactory.CreateRule(rule);

            // Assert
            result.Should().NotBeNull();
            result.ConditionNodeId.Should().Be(rule.ConditionNodeId);
            result.DateBegin.Should().Be(rule.DateBegin);
            result.DateEnd.Should().Be(rule.DateEnd);
            result.Name.Should().Be(rule.Name);
            result.Priority.Should().Be(rule.Priority);
        }

        [Fact]
        public void RuleFactory_CreateRule_CoreModel_Success()
        {
            // Arrange
            var rule = new Rule()
            {
                ConditionNode = null,
                ConditionNodeId = 1,
                Content = "Test",
                ContentType = new ContentType(),
                ContentTypeCode = 1,
                DateBegin = new DateTime(2023, 1, 1),
                DateEnd = new DateTime(2023, 2, 1),
                Name = "CarInsuranceAdvice",
                Priority = 1
            };

            var cenas = 1;

            var contentSerializer = new Mock<IContentSerializer>();
            contentSerializer.Setup(c => c.Deserialize(It.IsAny<object>(), It.IsAny<Type>()))
                .Returns(cenas);

            var contentSerializationProvider = new Mock<IContentSerializationProvider<MockContentType>>();
            contentSerializationProvider.Setup(x => x.GetContentSerializer(It.IsAny<MockContentType>()))
                .Returns(contentSerializer.Object);

            var ruleFactory = new RuleFactory<MockContentType, MockConditionType>(contentSerializationProvider.Object);

            var coreRule = ruleFactory.CreateRule(rule);

            // Act
            var result = ruleFactory.CreateRule(coreRule);

            // Assert
            result.Should().NotBeNull();
            result.ConditionNodeId.Should().Be(coreRule.ConditionNodeId);
            result.DateBegin.Should().Be(coreRule.DateBegin);
            result.DateEnd.Should().Be(coreRule.DateEnd);
            result.Name.Should().Be(coreRule.Name);
            result.Priority.Should().Be(coreRule.Priority);
        }

        [Fact]
        public void RuleFactory_CreateRule_CoreModel_ThrowsExcpetion()
        {
            // Arrange
            Core.Rule<MockContentType, MockConditionType> rule = null;

            var contentSerializationProvider = new Mock<IContentSerializationProvider<MockContentType>>();

            var ruleFactory = new RuleFactory<MockContentType, MockConditionType>(contentSerializationProvider.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ruleFactory.CreateRule(rule));
        }

        [Fact]
        public void RuleFactory_CreateRule_DataModel_ThrowsExcpetion()
        {
            // Arrange
            Rule rule = null;

            var contentSerializationProvider = new Mock<IContentSerializationProvider<MockContentType>>();

            var ruleFactory = new RuleFactory<MockContentType, MockConditionType>(contentSerializationProvider.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ruleFactory.CreateRule(rule));
        }

        [Fact]
        public void RuleFactory_CreateRule_InvalidRuleName_ThrowsInvalidRuleExcpetion()
        {
            // Arrange
            var rule = new Rule()
            {
                ConditionNode = new ConditionNode()
                {
                    LogicalOperator = new LogicalOperator()
                    {
                        Name = "Error",
                        Code = 3,
                        Symbol = "<",
                        ConditionNodes = new List<ConditionNode>()
                    },
                    LogicalOperatorCode = 3,
                    DataTypeCode = 1,
                    Id = 1,
                    Operand = "1",
                    Operator = new Operator()
                    {
                        Code = 1,
                        Name = "Test"
                    },
                    OperatorCode = 1,
                    ConditionType = new ConditionType() { Code = 1, Name = "Test" },
                    ConditionTypeCode = 0
                },
                ConditionNodeId = 1,
                Content = "Test",
                ContentType = new ContentType(),
                ContentTypeCode = 1,
                DateBegin = new DateTime(2023, 1, 1),
                DateEnd = new DateTime(2023, 2, 1),
                Name = "Test",
                Priority = 1
            };

            var contentSerializationProvider = new Mock<IContentSerializationProvider<MockContentType>>();

            var ruleFactory = new RuleFactory<MockContentType, MockConditionType>(contentSerializationProvider.Object);

            // Act & Assert
            Assert.Throws<InvalidRuleException>(() => ruleFactory.CreateRule(rule));
        }

        [Fact]
        public void RuleFactory_CreateRule_InvalidPriority_ThrowInvalidRuleException()
        {
            // Arrange
            var rule = new Rule()
            {
                ConditionNode = new ConditionNode()
                {
                    LogicalOperator = new LogicalOperator()
                    {
                        Name = "Eval",
                        Code = 3,
                        Symbol = "<",
                        ConditionNodes = new List<ConditionNode>()
                    },
                    LogicalOperatorCode = 3,
                    DataTypeCode = 1,
                    Id = 1,
                    Operand = "1",
                    Operator = new Operator()
                    {
                        Code = 1,
                        Name = "Test"
                    },
                    OperatorCode = 1,
                    ConditionType = new ConditionType() { Code = 1, Name = "Test" },
                    ConditionTypeCode = 1
                },
                ConditionNodeId = 1,
                Content = "Test",
                ContentType = new ContentType(),
                ContentTypeCode = 1,
                DateBegin = new DateTime(2023, 1, 1),
                DateEnd = new DateTime(2023, 2, 1),
                Name = "CarInsuranceAdvice",
                Priority = -1
            };

            var contentSerializationProvider = new Mock<IContentSerializationProvider<MockContentType>>();

            var ruleFactory = new RuleFactory<MockContentType, MockConditionType>(contentSerializationProvider.Object);

            // Act & Assert
            Assert.Throws<InvalidRuleException>(() => ruleFactory.CreateRule(rule));
        }
    }
}
