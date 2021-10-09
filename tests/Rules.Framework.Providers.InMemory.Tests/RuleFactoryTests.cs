namespace Rules.Framework.Providers.InMemory.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Providers.InMemory.DataModel;
    using Rules.Framework.Providers.InMemory.Tests.TestStubs;
    using Xunit;

    public class RuleFactoryTests
    {
        [Fact]
        public void CreateRule_GivenInvalidPriority_ThrowsInvalidRuleException()
        {
            // Arrange
            dynamic content = new ExpandoObject();
            content.Prop1 = 123;
            content.Prop2 = "Sample string";
            content.Prop3 = 500.34m;

            RuleDataModel<ContentType, ConditionType> ruleDataModel = new RuleDataModel<ContentType, ConditionType>
            {
                Content = content,
                ContentType = ContentType.ContentTypeSample,
                DateBegin = new DateTime(2020, 1, 1),
                DateEnd = null,
                Name = "My rule used for testing purposes",
                Priority = 0,
                RootCondition = null
            };

            RuleFactory<ContentType, ConditionType> ruleFactory = new RuleFactory<ContentType, ConditionType>();

            // Act
            InvalidRuleException invalidRuleException = Assert.Throws<InvalidRuleException>(() => ruleFactory.CreateRule(ruleDataModel));

            // Assert
            invalidRuleException.Should().NotBeNull();
            invalidRuleException.Errors.Should().NotBeEmpty();
            invalidRuleException.Errors.Should().Contain("Loaded rule priority number is invalid: 0.");
        }

        [Fact]
        public void CreateRule_GivenInvalidRuleName_ThrowsInvalidRuleException()
        {
            // Arrange
            dynamic content = new ExpandoObject();
            content.Prop1 = 123;
            content.Prop2 = "Sample string";
            content.Prop3 = 500.34m;

            RuleDataModel<ContentType, ConditionType> ruleDataModel = new RuleDataModel<ContentType, ConditionType>
            {
                Content = content,
                ContentType = ContentType.ContentTypeSample,
                DateBegin = new DateTime(2020, 1, 1),
                DateEnd = null,
                Name = null,
                Priority = 1,
                RootCondition = null
            };

            RuleFactory<ContentType, ConditionType> ruleFactory = new RuleFactory<ContentType, ConditionType>();

            // Act
            InvalidRuleException invalidRuleException = Assert.Throws<InvalidRuleException>(() => ruleFactory.CreateRule(ruleDataModel));

            // Assert
            invalidRuleException.Should().NotBeNull();
            invalidRuleException.Errors.Should().NotBeEmpty();
            invalidRuleException.Errors.Should().Contain("'Name' must not be empty.");
        }

        [Fact]
        public void CreateRule_GivenNullRule_ThrowsArgumentNullException()
        {
            // Arrange
            Rule<ContentType, ConditionType> rule = null;

            RuleFactory<ContentType, ConditionType> ruleFactory = new RuleFactory<ContentType, ConditionType>();

            // Act
            ArgumentNullException argumentNullException = Assert.Throws<ArgumentNullException>(() => ruleFactory.CreateRule(rule));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(rule));
        }

        [Fact]
        public void CreateRule_GivenNullRuleDataModel_ThrowsArgumentNullException()
        {
            // Arrange
            RuleDataModel<ContentType, ConditionType> ruleDataModel = null;

            RuleFactory<ContentType, ConditionType> ruleFactory = new RuleFactory<ContentType, ConditionType>();

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

            ValueConditionNodeDataModel<ConditionType> integerConditionNodeDataModel = new ValueConditionNodeDataModel<ConditionType>
            {
                ConditionType = ConditionType.SampleIntegerCondition,
                DataType = DataTypes.Integer,
                LogicalOperator = LogicalOperators.Eval,
                Operand = 20,
                Operator = Operators.GreaterThan
            };

            ValueConditionNodeDataModel<ConditionType> stringConditionNodeDataModel = new ValueConditionNodeDataModel<ConditionType>
            {
                ConditionType = ConditionType.SampleStringCondition,
                DataType = DataTypes.String,
                LogicalOperator = LogicalOperators.Eval,
                Operand = "TEST",
                Operator = Operators.Equal
            };

            ValueConditionNodeDataModel<ConditionType> decimalConditionNodeDataModel = new ValueConditionNodeDataModel<ConditionType>
            {
                ConditionType = ConditionType.SampleDecimalCondition,
                DataType = DataTypes.Decimal,
                LogicalOperator = LogicalOperators.Eval,
                Operand = 50.3m,
                Operator = Operators.LesserThanOrEqual
            };

            ValueConditionNodeDataModel<ConditionType> booleanConditionNodeDataModel = new ValueConditionNodeDataModel<ConditionType>
            {
                ConditionType = ConditionType.SampleBooleanCondition,
                DataType = DataTypes.Boolean,
                LogicalOperator = LogicalOperators.Eval,
                Operand = true,
                Operator = Operators.NotEqual
            };

            RuleDataModel<ContentType, ConditionType> ruleDataModel = new RuleDataModel<ContentType, ConditionType>
            {
                Content = content,
                ContentType = ContentType.ContentTypeSample,
                DateBegin = new System.DateTime(2020, 1, 1),
                DateEnd = null,
                Name = "My rule used for testing purposes",
                Priority = 1,
                RootCondition = new ComposedConditionNodeDataModel<ConditionType>
                {
                    LogicalOperator = LogicalOperators.And,
                    ChildConditionNodes = new ConditionNodeDataModel<ConditionType>[]
                    {
                        integerConditionNodeDataModel,
                        stringConditionNodeDataModel,
                        decimalConditionNodeDataModel,
                        booleanConditionNodeDataModel
                    }
                }
            };

            RuleFactory<ContentType, ConditionType> ruleFactory = new RuleFactory<ContentType, ConditionType>();

            // Act
            Rule<ContentType, ConditionType> rule = ruleFactory.CreateRule(ruleDataModel);

            // Assert
            rule.Should().NotBeNull();
            rule.ContentContainer.Should().NotBeNull()
                .And.BeOfType<ContentContainer<ContentType>>();
            rule.DateBegin.Should().Be(ruleDataModel.DateBegin);
            rule.DateEnd.Should().BeNull();
            rule.Name.Should().Be(ruleDataModel.Name);
            rule.Priority.Should().Be(ruleDataModel.Priority);
            rule.RootCondition.Should().BeOfType<ComposedConditionNode<ConditionType>>();

            ComposedConditionNode<ConditionType> composedConditionNode = rule.RootCondition.As<ComposedConditionNode<ConditionType>>();
            composedConditionNode.LogicalOperator.Should().Be(LogicalOperators.And);
            composedConditionNode.ChildConditionNodes.Should().HaveCount(4);

            IEnumerable<ValueConditionNode<ConditionType>> valueConditionNodes = composedConditionNode.ChildConditionNodes.OfType<ValueConditionNode<ConditionType>>();
            valueConditionNodes.Should().HaveCount(4);
            ValueConditionNode<ConditionType> integerConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.Integer);
            integerConditionNode.Should().NotBeNull();
            integerConditionNode.ConditionType.Should().Match(x => x == integerConditionNodeDataModel.ConditionType);
            integerConditionNode.DataType.Should().Be(integerConditionNodeDataModel.DataType);
            integerConditionNode.LogicalOperator.Should().Be(integerConditionNodeDataModel.LogicalOperator);
            integerConditionNode.Operand.Should().Match(x => object.Equals(x, integerConditionNodeDataModel.Operand));
            integerConditionNode.Operator.Should().Be(integerConditionNodeDataModel.Operator);

            ValueConditionNode<ConditionType> stringConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.String);
            stringConditionNode.Should().NotBeNull();
            stringConditionNode.ConditionType.Should().Match(x => x == stringConditionNodeDataModel.ConditionType);
            stringConditionNode.DataType.Should().Be(stringConditionNodeDataModel.DataType);
            stringConditionNode.LogicalOperator.Should().Be(stringConditionNodeDataModel.LogicalOperator);
            stringConditionNode.Operand.Should().Match(x => object.Equals(x, stringConditionNodeDataModel.Operand));
            stringConditionNode.Operator.Should().Be(stringConditionNodeDataModel.Operator);

            ValueConditionNode<ConditionType> decimalConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.Decimal);
            decimalConditionNode.Should().NotBeNull();
            decimalConditionNode.ConditionType.Should().Match(x => x == decimalConditionNodeDataModel.ConditionType);
            decimalConditionNode.DataType.Should().Be(decimalConditionNodeDataModel.DataType);
            decimalConditionNode.LogicalOperator.Should().Be(decimalConditionNodeDataModel.LogicalOperator);
            decimalConditionNode.Operand.Should().Match(x => object.Equals(x, decimalConditionNodeDataModel.Operand));
            decimalConditionNode.Operator.Should().Be(decimalConditionNodeDataModel.Operator);

            ValueConditionNode<ConditionType> booleanConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.Boolean);
            booleanConditionNode.Should().NotBeNull();
            booleanConditionNode.ConditionType.Should().Match(x => x == booleanConditionNodeDataModel.ConditionType);
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

            ValueConditionNode<ConditionType> booleanConditionNode = null;
            ValueConditionNode<ConditionType> decimalConditionNode = null;
            ValueConditionNode<ConditionType> integerConditionNode = null;
            ValueConditionNode<ConditionType> stringConditionNode = null;

            Rule<ContentType, ConditionType> rule1 = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName("My rule used for testing purposes")
                .WithDateBegin(new DateTime(2020, 1, 1))
                .WithContentContainer(new ContentContainer<ContentType>(ContentType.ContentTypeSample, (t) => (object)content))
                .WithCondition(cnb => cnb.AsComposed()
                    .WithLogicalOperator(LogicalOperators.And)
                    .AddCondition(cnb1 => booleanConditionNode = cnb1.AsValued(ConditionType.SampleBooleanCondition)
                        .OfDataType<bool>()
                        .WithComparisonOperator(Operators.NotEqual)
                        .SetOperand(true)
                        .Build() as ValueConditionNode<ConditionType>)
                    .AddCondition(cnb1 => decimalConditionNode = cnb1.AsValued(ConditionType.SampleDecimalCondition)
                        .OfDataType<decimal>()
                        .WithComparisonOperator(Operators.LesserThanOrEqual)
                        .SetOperand(50.3m)
                        .Build() as ValueConditionNode<ConditionType>)
                    .AddCondition(cnb1 => integerConditionNode = cnb1.AsValued(ConditionType.SampleIntegerCondition)
                        .OfDataType<int>()
                        .WithComparisonOperator(Operators.GreaterThan)
                        .SetOperand(20)
                        .Build() as ValueConditionNode<ConditionType>)
                    .AddCondition(cnb1 => stringConditionNode = cnb1.AsValued(ConditionType.SampleStringCondition)
                        .OfDataType<string>()
                        .WithComparisonOperator(Operators.Equal)
                        .SetOperand("TEST")
                        .Build() as ValueConditionNode<ConditionType>)
                    .Build())
                .Build().Rule;

            RuleFactory<ContentType, ConditionType> ruleFactory = new RuleFactory<ContentType, ConditionType>();

            // Act
            RuleDataModel<ContentType, ConditionType> rule = ruleFactory.CreateRule(rule1);

            // Assert
            rule.Should().NotBeNull();
            object content1 = rule.Content;
            content1.Should().NotBeNull()
                .And.BeSameAs(content);
            rule.DateBegin.Should().Be(rule.DateBegin);
            rule.DateEnd.Should().BeNull();
            rule.Name.Should().Be(rule.Name);
            rule.Priority.Should().Be(rule.Priority);
            rule.RootCondition.Should().BeOfType<ComposedConditionNodeDataModel<ConditionType>>();

            ComposedConditionNodeDataModel<ConditionType> composedConditionNodeDataModel = rule.RootCondition.As<ComposedConditionNodeDataModel<ConditionType>>();
            composedConditionNodeDataModel.LogicalOperator.Should().Be(LogicalOperators.And);
            composedConditionNodeDataModel.ChildConditionNodes.Should().HaveCount(4);

            IEnumerable<ValueConditionNodeDataModel<ConditionType>> valueConditionNodeDataModels = composedConditionNodeDataModel.ChildConditionNodes.OfType<ValueConditionNodeDataModel<ConditionType>>();
            valueConditionNodeDataModels.Should().HaveCount(4);
            ValueConditionNodeDataModel<ConditionType> integerConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Integer);
            integerConditionNodeDataModel.Should().NotBeNull();
            integerConditionNodeDataModel.ConditionType.Should().Match(x => integerConditionNode.ConditionType == x);
            integerConditionNodeDataModel.DataType.Should().Be(integerConditionNode.DataType);
            integerConditionNodeDataModel.LogicalOperator.Should().Be(integerConditionNode.LogicalOperator);
            integerConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, integerConditionNode.Operand));
            integerConditionNodeDataModel.Operator.Should().Be(integerConditionNode.Operator);

            ValueConditionNodeDataModel<ConditionType> stringConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.String);
            stringConditionNodeDataModel.Should().NotBeNull();
            stringConditionNodeDataModel.ConditionType.Should().Match(x => stringConditionNode.ConditionType == x);
            stringConditionNodeDataModel.DataType.Should().Be(stringConditionNode.DataType);
            stringConditionNodeDataModel.LogicalOperator.Should().Be(stringConditionNode.LogicalOperator);
            stringConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, stringConditionNode.Operand));
            stringConditionNodeDataModel.Operator.Should().Be(stringConditionNode.Operator);

            ValueConditionNodeDataModel<ConditionType> decimalConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Decimal);
            decimalConditionNodeDataModel.Should().NotBeNull();
            decimalConditionNodeDataModel.ConditionType.Should().Match(x => decimalConditionNode.ConditionType == x);
            decimalConditionNodeDataModel.DataType.Should().Be(decimalConditionNode.DataType);
            decimalConditionNodeDataModel.LogicalOperator.Should().Be(decimalConditionNode.LogicalOperator);
            decimalConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, decimalConditionNode.Operand));
            decimalConditionNodeDataModel.Operator.Should().Be(decimalConditionNode.Operator);

            ValueConditionNodeDataModel<ConditionType> booleanConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Boolean);
            booleanConditionNodeDataModel.Should().NotBeNull();
            booleanConditionNodeDataModel.ConditionType.Should().Match(x => booleanConditionNode.ConditionType == x);
            booleanConditionNodeDataModel.DataType.Should().Be(booleanConditionNode.DataType);
            booleanConditionNodeDataModel.LogicalOperator.Should().Be(booleanConditionNode.LogicalOperator);
            booleanConditionNodeDataModel.Operand.Should().Be(Convert.ToBoolean(booleanConditionNode.Operand));
            booleanConditionNodeDataModel.Operator.Should().Be(booleanConditionNode.Operator);
        }
    }
}