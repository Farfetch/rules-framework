namespace Rules.Framework.Providers.MongoDb.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using FluentAssertions;
    using MongoDB.Bson;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Providers.MongoDb.DataModel;
    using Rules.Framework.Providers.MongoDb.Tests.TestStubs;
    using Rules.Framework.Serialization;
    using Xunit;

    public class RuleFactoryTests
    {
        [Fact]
        public void CreateRule_GivenNullRuleDataModel_ThrowsArgumentNullException()
        {
            // Arrange
            RuleDataModel ruleDataModel = null;

            IContentSerializationProvider<ContentType> contentSerializationProvider = Mock.Of<IContentSerializationProvider<ContentType>>();

            RuleFactory<ContentType, ConditionType> ruleFactory = new RuleFactory<ContentType, ConditionType>(contentSerializationProvider);

            // Act
            ArgumentNullException argumentNullException = Assert.Throws<ArgumentNullException>(() => ruleFactory.CreateRule(ruleDataModel));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(ruleDataModel));
        }

        [Fact]
        public void CreateRule_GivenRuleDataModelWithComposedNodeAndChildNodesOfEachDataType_ReturnsRuleInstance()
        {
            // Arrange
            dynamic content = new ExpandoObject();
            content.Prop1 = 123;
            content.Prop2 = "Sample string";
            content.Prop3 = 500.34m;

            ValueConditionNodeDataModel integerConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                ConditionType = "SampleIntegerCondition",
                DataType = DataTypes.Integer,
                LogicalOperator = LogicalOperators.Eval,
                Operand = 20,
                Operator = Operators.GreaterThan
            };

            ValueConditionNodeDataModel stringConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                ConditionType = "SampleStringCondition",
                DataType = DataTypes.String,
                LogicalOperator = LogicalOperators.Eval,
                Operand = "TEST",
                Operator = Operators.Equal
            };

            ValueConditionNodeDataModel decimalConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                ConditionType = "SampleDecimalCondition",
                DataType = DataTypes.Decimal,
                LogicalOperator = LogicalOperators.Eval,
                Operand = 50.3m,
                Operator = Operators.LesserThanOrEqual
            };

            ValueConditionNodeDataModel booleanConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                ConditionType = "SampleBooleanCondition",
                DataType = DataTypes.Boolean,
                LogicalOperator = LogicalOperators.Eval,
                Operand = true,
                Operator = Operators.NotEqual
            };

            RuleDataModel ruleDataModel = new RuleDataModel
            {
                Content = content,
                ContentType = "ContentTypeSample",
                DateBegin = new System.DateTime(2020, 1, 1),
                DateEnd = null,
                Id = ObjectId.GenerateNewId(),
                Name = "My rule used for testing purposes",
                Priority = 1,
                RootCondition = new ComposedConditionNodeDataModel
                {
                    LogicalOperator = LogicalOperators.And,
                    ChildConditionNodes = new ConditionNodeDataModel[]
                    {
                        integerConditionNodeDataModel,
                        stringConditionNodeDataModel,
                        decimalConditionNodeDataModel,
                        booleanConditionNodeDataModel
                    }
                }
            };

            IContentSerializationProvider<ContentType> contentSerializationProvider = Mock.Of<IContentSerializationProvider<ContentType>>();

            RuleFactory<ContentType, ConditionType> ruleFactory = new RuleFactory<ContentType, ConditionType>(contentSerializationProvider);

            // Act
            Rule<ContentType, ConditionType> rule = ruleFactory.CreateRule(ruleDataModel);

            // Assert
            rule.Should().NotBeNull();
            rule.ContentContainer.Should().NotBeNull()
                .And.BeOfType<SerializedContentContainer<ContentType>>();
            rule.DateBegin.Should().Be(ruleDataModel.DateBegin);
            rule.DateEnd.Should().BeNull();
            rule.Name.Should().Be(ruleDataModel.Name);
            rule.Priority.Should().Be(ruleDataModel.Priority);
            rule.RootCondition.Should().BeOfType<ComposedConditionNode<ConditionType>>();

            ComposedConditionNode<ConditionType> composedConditionNode = rule.RootCondition.As<ComposedConditionNode<ConditionType>>();
            composedConditionNode.LogicalOperator.Should().Be(LogicalOperators.And);
            composedConditionNode.ChildConditionNodes.Should().HaveCount(4);

            IEnumerable<IntegerConditionNode<ConditionType>> integerConditionNodes = composedConditionNode.ChildConditionNodes.OfType<IntegerConditionNode<ConditionType>>();
            integerConditionNodes.Should().HaveCount(1);
            IntegerConditionNode<ConditionType> integerConditionNode = integerConditionNodes.First();
            integerConditionNode.Should().NotBeNull();
            integerConditionNode.ConditionType.Should().Match<ConditionType>(x => x == Enum.Parse<ConditionType>(integerConditionNodeDataModel.ConditionType));
            integerConditionNode.DataType.Should().Be(integerConditionNodeDataModel.DataType);
            integerConditionNode.LogicalOperator.Should().Be(integerConditionNodeDataModel.LogicalOperator);
            integerConditionNode.Operand.Should().Match(x => object.Equals(x, integerConditionNodeDataModel.Operand));
            integerConditionNode.Operator.Should().Be(integerConditionNodeDataModel.Operator);

            IEnumerable<StringConditionNode<ConditionType>> stringConditionNodes = composedConditionNode.ChildConditionNodes.OfType<StringConditionNode<ConditionType>>();
            stringConditionNodes.Should().HaveCount(1);
            StringConditionNode<ConditionType> stringConditionNode = stringConditionNodes.First();
            stringConditionNode.Should().NotBeNull();
            stringConditionNode.ConditionType.Should().Match<ConditionType>(x => x == Enum.Parse<ConditionType>(stringConditionNodeDataModel.ConditionType));
            stringConditionNode.DataType.Should().Be(stringConditionNodeDataModel.DataType);
            stringConditionNode.LogicalOperator.Should().Be(stringConditionNodeDataModel.LogicalOperator);
            stringConditionNode.Operand.Should().Match(x => object.Equals(x, stringConditionNodeDataModel.Operand));
            stringConditionNode.Operator.Should().Be(stringConditionNodeDataModel.Operator);

            IEnumerable<DecimalConditionNode<ConditionType>> decimalConditionNodes = composedConditionNode.ChildConditionNodes.OfType<DecimalConditionNode<ConditionType>>();
            decimalConditionNodes.Should().HaveCount(1);
            DecimalConditionNode<ConditionType> decimalConditionNode = decimalConditionNodes.First();
            decimalConditionNode.Should().NotBeNull();
            decimalConditionNode.ConditionType.Should().Match<ConditionType>(x => x == Enum.Parse<ConditionType>(decimalConditionNodeDataModel.ConditionType));
            decimalConditionNode.DataType.Should().Be(decimalConditionNodeDataModel.DataType);
            decimalConditionNode.LogicalOperator.Should().Be(decimalConditionNodeDataModel.LogicalOperator);
            decimalConditionNode.Operand.Should().Match(x => object.Equals(x, decimalConditionNodeDataModel.Operand));
            decimalConditionNode.Operator.Should().Be(decimalConditionNodeDataModel.Operator);

            IEnumerable<BooleanConditionNode<ConditionType>> booleanConditionNodes = composedConditionNode.ChildConditionNodes.OfType<BooleanConditionNode<ConditionType>>();
            booleanConditionNodes.Should().HaveCount(1);
            BooleanConditionNode<ConditionType> booleanConditionNode = booleanConditionNodes.First();
            booleanConditionNode.Should().NotBeNull();
            booleanConditionNode.ConditionType.Should().Match<ConditionType>(x => x == Enum.Parse<ConditionType>(booleanConditionNodeDataModel.ConditionType));
            booleanConditionNode.DataType.Should().Be(booleanConditionNodeDataModel.DataType);
            booleanConditionNode.LogicalOperator.Should().Be(booleanConditionNodeDataModel.LogicalOperator);
            booleanConditionNode.Operand.Should().Be(Convert.ToBoolean(booleanConditionNodeDataModel.Operand));
            booleanConditionNode.Operator.Should().Be(booleanConditionNodeDataModel.Operator);
        }
    }
}