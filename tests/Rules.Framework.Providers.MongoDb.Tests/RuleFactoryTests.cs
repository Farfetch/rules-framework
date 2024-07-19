namespace Rules.Framework.Providers.MongoDb.Tests
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using FluentAssertions;
    using Moq;
    using Rules.Framework;
    using Rules.Framework.Builder;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Providers.MongoDb.DataModel;
    using Rules.Framework.Providers.MongoDb.Tests.TestStubs;
    using Rules.Framework.Serialization;
    using Xunit;

    public class RuleFactoryTests
    {
        [Fact]
        public void CreateRule_GivenNullRule_ThrowsArgumentNullException()
        {
            // Arrange
            Rule rule = null;

            var contentSerializationProvider = Mock.Of<IContentSerializationProvider>();

            var ruleFactory = new RuleFactory(contentSerializationProvider);

            // Act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => ruleFactory.CreateRule(rule));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(rule));
        }

        [Fact]
        public void CreateRule_GivenNullRuleDataModel_ThrowsArgumentNullException()
        {
            // Arrange
            RuleDataModel ruleDataModel = null;

            var contentSerializationProvider = Mock.Of<IContentSerializationProvider>();

            var ruleFactory = new RuleFactory(contentSerializationProvider);

            // Act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => ruleFactory.CreateRule(ruleDataModel));

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

            var integerConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                Condition = "SampleIntegerCondition",
                DataType = DataTypes.Integer,
                LogicalOperator = LogicalOperators.Eval,
                Operand = 20,
                Operator = Operators.GreaterThan
            };

            var stringConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                Condition = "SampleStringCondition",
                DataType = DataTypes.String,
                LogicalOperator = LogicalOperators.Eval,
                Operand = "TEST",
                Operator = Operators.Equal
            };

            var decimalConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                Condition = "SampleDecimalCondition",
                DataType = DataTypes.Decimal,
                LogicalOperator = LogicalOperators.Eval,
                Operand = 50.3m,
                Operator = Operators.LesserThanOrEqual
            };

            var booleanConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                Condition = "SampleBooleanCondition",
                DataType = DataTypes.Boolean,
                LogicalOperator = LogicalOperators.Eval,
                Operand = true,
                Operator = Operators.NotEqual
            };

            var ruleDataModel = new RuleDataModel
            {
                Content = content,
                Ruleset = "ContentTypeSample",
                DateBegin = new System.DateTime(2020, 1, 1),
                DateEnd = null,
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

            var contentSerializationProvider = Mock.Of<IContentSerializationProvider>();

            var ruleFactory = new RuleFactory(contentSerializationProvider);

            // Act
            var rule = ruleFactory.CreateRule(ruleDataModel);

            // Assert
            rule.Should().NotBeNull();
            rule.ContentContainer.Should().NotBeNull().And.BeOfType<SerializedContentContainer>();
            rule.DateBegin.Should().Be(ruleDataModel.DateBegin);
            rule.DateEnd.Should().BeNull();
            rule.Name.Should().Be(ruleDataModel.Name);
            rule.Priority.Should().Be(ruleDataModel.Priority);
            rule.Ruleset.Should().Be(ruleDataModel.Ruleset);
            rule.RootCondition.Should().BeOfType<ComposedConditionNode>();

            var composedConditionNode = rule.RootCondition.As<ComposedConditionNode>();
            composedConditionNode.LogicalOperator.Should().Be(LogicalOperators.And);
            composedConditionNode.ChildConditionNodes.Should().HaveCount(4);

            var valueConditionNodes = composedConditionNode.ChildConditionNodes.OfType<ValueConditionNode>();
            valueConditionNodes.Should().HaveCount(4);

            var integerConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.Integer);
            integerConditionNode.Should().NotBeNull();
            integerConditionNode.Condition.Should().Be(integerConditionNodeDataModel.Condition);
            integerConditionNode.DataType.Should().Be(integerConditionNodeDataModel.DataType);
            integerConditionNode.LogicalOperator.Should().Be(integerConditionNodeDataModel.LogicalOperator);
            integerConditionNode.Operand.Should().Match(x => object.Equals(x, integerConditionNodeDataModel.Operand));
            integerConditionNode.Operator.Should().Be(integerConditionNodeDataModel.Operator);

            var stringConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.String);
            stringConditionNode.Should().NotBeNull();
            stringConditionNode.Condition.Should().Be(stringConditionNodeDataModel.Condition);
            stringConditionNode.DataType.Should().Be(stringConditionNodeDataModel.DataType);
            stringConditionNode.LogicalOperator.Should().Be(stringConditionNodeDataModel.LogicalOperator);
            stringConditionNode.Operand.Should().Match(x => object.Equals(x, stringConditionNodeDataModel.Operand));
            stringConditionNode.Operator.Should().Be(stringConditionNodeDataModel.Operator);

            var decimalConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.Decimal);
            decimalConditionNode.Should().NotBeNull();
            decimalConditionNode.Condition.Should().Be(decimalConditionNodeDataModel.Condition);
            decimalConditionNode.DataType.Should().Be(decimalConditionNodeDataModel.DataType);
            decimalConditionNode.LogicalOperator.Should().Be(decimalConditionNodeDataModel.LogicalOperator);
            decimalConditionNode.Operand.Should().Match(x => object.Equals(x, decimalConditionNodeDataModel.Operand));
            decimalConditionNode.Operator.Should().Be(decimalConditionNodeDataModel.Operator);

            var booleanConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.Boolean);
            booleanConditionNode.Should().NotBeNull();
            booleanConditionNode.Condition.Should().Be(booleanConditionNodeDataModel.Condition);
            booleanConditionNode.DataType.Should().Be(booleanConditionNodeDataModel.DataType);
            booleanConditionNode.LogicalOperator.Should().Be(booleanConditionNodeDataModel.LogicalOperator);
            booleanConditionNode.Operand.Should().Be(Convert.ToBoolean(booleanConditionNodeDataModel.Operand));
            booleanConditionNode.Operator.Should().Be(booleanConditionNodeDataModel.Operator);
        }

        [Fact]
        public void CreateRule_GivenRuleWithComposedNodeAndChildNodesOfEachDataType_ReturnsRuleDataModelInstance()
        {
            // Arrange
            dynamic content = new ExpandoObject();
            content.Prop1 = 123;
            content.Prop2 = "Sample string";
            content.Prop3 = 500.34m;

            var contentSerializer = Mock.Of<IContentSerializer>();
            Mock.Get(contentSerializer)
                .Setup(x => x.Deserialize(It.IsAny<object>(), It.IsAny<Type>()))
                .Returns((object)content);
            Mock.Get(contentSerializer)
                .Setup(x => x.Serialize(It.IsAny<object>()))
                .Returns((object)content);

            var contentSerializationProvider = Mock.Of<IContentSerializationProvider>();
            Mock.Get(contentSerializationProvider)
                .Setup(x => x.GetContentSerializer(RulesetNames.RulesetSample.ToString()))
                .Returns(contentSerializer);

            var booleanConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionNames.SampleBooleanCondition.ToString(), Operators.NotEqual, true);
            var decimalConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionNames.SampleDecimalCondition.ToString(), Operators.LesserThanOrEqual, 50.3m);
            var integerConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionNames.SampleIntegerCondition.ToString(), Operators.GreaterThan, 20);
            var stringConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionNames.SampleStringCondition.ToString(), Operators.Equal, "TEST");

            var rule = Rule.Create<RulesetNames, ConditionNames>("My rule used for testing purposes")
                .OnRuleset(RulesetNames.RulesetSample)
                .SetContent((object)content)
                .Since(new DateTime(2020, 1, 1))
                .ApplyWhen(c => c
                    .And(a => a
                        .Condition(booleanConditionNode)
                        .Condition(decimalConditionNode)
                        .Condition(integerConditionNode)
                        .Condition(stringConditionNode)
                    ))
                .Build().Rule;

            var ruleFactory = new RuleFactory(contentSerializationProvider);

            // Act
            var ruleDataModel = ruleFactory.CreateRule(rule);

            // Assert
            ruleDataModel.Should().NotBeNull();
            var ruleDataModelContent = rule.ContentContainer.GetContentAs<object>();
            ruleDataModelContent.Should().NotBeNull().And.BeSameAs(content);
            ruleDataModel.DateBegin.Should().Be(rule.DateBegin);
            ruleDataModel.DateEnd.Should().BeNull();
            ruleDataModel.Name.Should().Be(rule.Name);
            ruleDataModel.Priority.Should().Be(rule.Priority);
            ruleDataModel.Ruleset.Should().Be(rule.Ruleset.ToString());
            ruleDataModel.RootCondition.Should().BeOfType<ComposedConditionNodeDataModel>();

            var composedConditionNodeDataModel = ruleDataModel.RootCondition.As<ComposedConditionNodeDataModel>();
            composedConditionNodeDataModel.LogicalOperator.Should().Be(LogicalOperators.And);
            composedConditionNodeDataModel.ChildConditionNodes.Should().HaveCount(4);

            var valueConditionNodeDataModels = composedConditionNodeDataModel.ChildConditionNodes.OfType<ValueConditionNodeDataModel>();
            valueConditionNodeDataModels.Should().HaveCount(4);

            var integerConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Integer);
            integerConditionNodeDataModel.Should().NotBeNull();
            integerConditionNodeDataModel.Condition.Should().Be(integerConditionNode.Condition);
            integerConditionNodeDataModel.DataType.Should().Be(integerConditionNode.DataType);
            integerConditionNodeDataModel.LogicalOperator.Should().Be(integerConditionNode.LogicalOperator);
            integerConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, integerConditionNode.Operand));
            integerConditionNodeDataModel.Operator.Should().Be(integerConditionNode.Operator);

            var stringConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.String);
            stringConditionNodeDataModel.Should().NotBeNull();
            stringConditionNodeDataModel.Condition.Should().Be(stringConditionNode.Condition);
            stringConditionNodeDataModel.DataType.Should().Be(stringConditionNode.DataType);
            stringConditionNodeDataModel.LogicalOperator.Should().Be(stringConditionNode.LogicalOperator);
            stringConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, stringConditionNode.Operand));
            stringConditionNodeDataModel.Operator.Should().Be(stringConditionNode.Operator);

            var decimalConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Decimal);
            decimalConditionNodeDataModel.Should().NotBeNull();
            decimalConditionNodeDataModel.Condition.Should().Be(decimalConditionNode.Condition);
            decimalConditionNodeDataModel.DataType.Should().Be(decimalConditionNode.DataType);
            decimalConditionNodeDataModel.LogicalOperator.Should().Be(decimalConditionNode.LogicalOperator);
            decimalConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, decimalConditionNode.Operand));
            decimalConditionNodeDataModel.Operator.Should().Be(decimalConditionNode.Operator);

            var booleanConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Boolean);
            booleanConditionNodeDataModel.Should().NotBeNull();
            booleanConditionNodeDataModel.Condition.Should().Be(booleanConditionNode.Condition);
            booleanConditionNodeDataModel.DataType.Should().Be(booleanConditionNode.DataType);
            booleanConditionNodeDataModel.LogicalOperator.Should().Be(booleanConditionNode.LogicalOperator);
            booleanConditionNodeDataModel.Operand.Should().Be(Convert.ToBoolean(booleanConditionNode.Operand));
            booleanConditionNodeDataModel.Operator.Should().Be(booleanConditionNode.Operator);
        }
    }
}