namespace Rules.Framework.Tests.Providers.InMemory
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using FluentAssertions;
    using Rules.Framework;
    using Rules.Framework.Builder;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Core;
    using Rules.Framework.Providers.InMemory;
    using Rules.Framework.Providers.InMemory.DataModel;
    using Rules.Framework.Tests.Providers.InMemory.TestStubs;
    using Xunit;

    public class RuleFactoryTests
    {
        [Fact]
        public void CreateRule_GivenNullRule_ThrowsArgumentNullException()
        {
            // Arrange
            Rule rule = null;

            var ruleFactory = new RuleFactory();

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

            var ruleFactory = new RuleFactory();

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
                Condition = ConditionNames.SampleIntegerCondition.ToString(),
                DataType = DataTypes.Integer,
                LogicalOperator = LogicalOperators.Eval,
                Operand = 20,
                Operator = Operators.GreaterThan,
                Properties = new PropertiesDictionary(2),
            };

            var stringConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                Condition = ConditionNames.SampleStringCondition.ToString(),
                DataType = DataTypes.String,
                LogicalOperator = LogicalOperators.Eval,
                Operand = "TEST",
                Operator = Operators.Equal,
                Properties = new PropertiesDictionary(2),
            };

            var decimalConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                Condition = ConditionNames.SampleDecimalCondition.ToString(),
                DataType = DataTypes.Decimal,
                LogicalOperator = LogicalOperators.Eval,
                Operand = 50.3m,
                Operator = Operators.LesserThanOrEqual,
                Properties = new PropertiesDictionary(2),
            };

            var booleanConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                Condition = ConditionNames.SampleBooleanCondition.ToString(),
                DataType = DataTypes.Boolean,
                LogicalOperator = LogicalOperators.Eval,
                Operand = true,
                Operator = Operators.NotEqual,
                Properties = new PropertiesDictionary(2),
            };

            var ruleDataModel = new RuleDataModel
            {
                Content = content,
                Ruleset = RulesetNames.ContentTypeSample.ToString(),
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
                    },
                    Properties = new PropertiesDictionary(2),
                }
            };

            var ruleFactory = new RuleFactory();

            // Act
            var rule = ruleFactory.CreateRule(ruleDataModel);

            // Assert
            rule.Should().NotBeNull();
            rule.ContentContainer.Should().NotBeNull()
                .And.BeOfType<ContentContainer>();
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
            integerConditionNode.Condition.Should().Match(x => x == integerConditionNodeDataModel.Condition);
            integerConditionNode.DataType.Should().Be(integerConditionNodeDataModel.DataType);
            integerConditionNode.LogicalOperator.Should().Be(integerConditionNodeDataModel.LogicalOperator);
            integerConditionNode.Operand.Should().Match(x => object.Equals(x, integerConditionNodeDataModel.Operand));
            integerConditionNode.Operator.Should().Be(integerConditionNodeDataModel.Operator);

            var stringConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.String);
            stringConditionNode.Should().NotBeNull();
            stringConditionNode.Condition.Should().Match(x => x == stringConditionNodeDataModel.Condition);
            stringConditionNode.DataType.Should().Be(stringConditionNodeDataModel.DataType);
            stringConditionNode.LogicalOperator.Should().Be(stringConditionNodeDataModel.LogicalOperator);
            stringConditionNode.Operand.Should().Match(x => object.Equals(x, stringConditionNodeDataModel.Operand));
            stringConditionNode.Operator.Should().Be(stringConditionNodeDataModel.Operator);

            var decimalConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.Decimal);
            decimalConditionNode.Should().NotBeNull();
            decimalConditionNode.Condition.Should().Match(x => x == decimalConditionNodeDataModel.Condition);
            decimalConditionNode.DataType.Should().Be(decimalConditionNodeDataModel.DataType);
            decimalConditionNode.LogicalOperator.Should().Be(decimalConditionNodeDataModel.LogicalOperator);
            decimalConditionNode.Operand.Should().Match(x => object.Equals(x, decimalConditionNodeDataModel.Operand));
            decimalConditionNode.Operator.Should().Be(decimalConditionNodeDataModel.Operator);

            var booleanConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.Boolean);
            booleanConditionNode.Should().NotBeNull();
            booleanConditionNode.Condition.Should().Match(x => x == booleanConditionNodeDataModel.Condition);
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

            var booleanConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionNames.SampleBooleanCondition.ToString(), Operators.NotEqual, true) as ValueConditionNode;
            var decimalConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionNames.SampleDecimalCondition.ToString(), Operators.LesserThanOrEqual, 50.3m) as ValueConditionNode;
            var integerConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionNames.SampleIntegerCondition.ToString(), Operators.GreaterThan, 20) as ValueConditionNode;
            var stringConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionNames.SampleStringCondition.ToString(), Operators.Equal, "TEST") as ValueConditionNode;

            var rule1 = Rule.Create<RulesetNames, ConditionNames>("My rule used for testing purposes")
                .InRuleset(RulesetNames.ContentTypeSample)
                .SetContent((object)content)
                .Since(new DateTime(2020, 1, 1))
                .ApplyWhen(c => c
                    .And(a => a
                        .Condition(booleanConditionNode)
                        .Condition(decimalConditionNode)
                        .Condition(integerConditionNode)
                        .Condition(stringConditionNode)
                    )
                )
                .Build().Rule;

            var ruleFactory = new RuleFactory();

            // Act
            var rule = ruleFactory.CreateRule(rule1);

            // Assert
            rule.Should().NotBeNull();
            object content1 = rule.Content;
            content1.Should().NotBeNull()
                .And.BeSameAs(content);
            rule.DateBegin.Should().Be(rule.DateBegin);
            rule.DateEnd.Should().BeNull();
            rule.Name.Should().Be(rule.Name);
            rule.Priority.Should().Be(rule.Priority);
            rule.Ruleset.Should().Be(rule.Ruleset);
            rule.RootCondition.Should().BeOfType<ComposedConditionNodeDataModel>();

            var composedConditionNodeDataModel = rule.RootCondition.As<ComposedConditionNodeDataModel>();
            composedConditionNodeDataModel.LogicalOperator.Should().Be(LogicalOperators.And);
            composedConditionNodeDataModel.ChildConditionNodes.Should().HaveCount(4);

            var valueConditionNodeDataModels = composedConditionNodeDataModel.ChildConditionNodes.OfType<ValueConditionNodeDataModel>();
            valueConditionNodeDataModels.Should().HaveCount(4);
            var integerConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Integer);
            integerConditionNodeDataModel.Should().NotBeNull();
            integerConditionNodeDataModel.Condition.Should().Match(x => integerConditionNode.Condition == x);
            integerConditionNodeDataModel.DataType.Should().Be(integerConditionNode.DataType);
            integerConditionNodeDataModel.LogicalOperator.Should().Be(integerConditionNode.LogicalOperator);
            integerConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, integerConditionNode.Operand));
            integerConditionNodeDataModel.Operator.Should().Be(integerConditionNode.Operator);

            var stringConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.String);
            stringConditionNodeDataModel.Should().NotBeNull();
            stringConditionNodeDataModel.Condition.Should().Match(x => stringConditionNode.Condition == x);
            stringConditionNodeDataModel.DataType.Should().Be(stringConditionNode.DataType);
            stringConditionNodeDataModel.LogicalOperator.Should().Be(stringConditionNode.LogicalOperator);
            stringConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, stringConditionNode.Operand));
            stringConditionNodeDataModel.Operator.Should().Be(stringConditionNode.Operator);

            var decimalConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Decimal);
            decimalConditionNodeDataModel.Should().NotBeNull();
            decimalConditionNodeDataModel.Condition.Should().Match(x => decimalConditionNode.Condition == x);
            decimalConditionNodeDataModel.DataType.Should().Be(decimalConditionNode.DataType);
            decimalConditionNodeDataModel.LogicalOperator.Should().Be(decimalConditionNode.LogicalOperator);
            decimalConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, decimalConditionNode.Operand));
            decimalConditionNodeDataModel.Operator.Should().Be(decimalConditionNode.Operator);

            var booleanConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Boolean);
            booleanConditionNodeDataModel.Should().NotBeNull();
            booleanConditionNodeDataModel.Condition.Should().Match(x => booleanConditionNode.Condition == x);
            booleanConditionNodeDataModel.DataType.Should().Be(booleanConditionNode.DataType);
            booleanConditionNodeDataModel.LogicalOperator.Should().Be(booleanConditionNode.LogicalOperator);
            booleanConditionNodeDataModel.Operand.Should().Be(Convert.ToBoolean(booleanConditionNode.Operand));
            booleanConditionNodeDataModel.Operator.Should().Be(booleanConditionNode.Operator);
        }
    }
}