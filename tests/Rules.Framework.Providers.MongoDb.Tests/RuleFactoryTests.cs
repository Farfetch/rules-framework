namespace Rules.Framework.Providers.MongoDb.Tests
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
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
            Rule<ContentType, ConditionType> rule = null;

            var contentSerializationProvider = Mock.Of<IContentSerializationProvider<ContentType>>();

            var ruleFactory = new RuleFactory<ContentType, ConditionType>(contentSerializationProvider);

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

            var contentSerializationProvider = Mock.Of<IContentSerializationProvider<ContentType>>();

            var ruleFactory = new RuleFactory<ContentType, ConditionType>(contentSerializationProvider);

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
                ConditionType = "SampleIntegerCondition",
                DataType = DataTypes.Integer,
                LogicalOperator = LogicalOperators.Eval,
                Operand = 20,
                Operator = Operators.GreaterThan
            };

            var stringConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                ConditionType = "SampleStringCondition",
                DataType = DataTypes.String,
                LogicalOperator = LogicalOperators.Eval,
                Operand = "TEST",
                Operator = Operators.Equal
            };

            var decimalConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                ConditionType = "SampleDecimalCondition",
                DataType = DataTypes.Decimal,
                LogicalOperator = LogicalOperators.Eval,
                Operand = 50.3m,
                Operator = Operators.LesserThanOrEqual
            };

            var booleanConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                ConditionType = "SampleBooleanCondition",
                DataType = DataTypes.Boolean,
                LogicalOperator = LogicalOperators.Eval,
                Operand = true,
                Operator = Operators.NotEqual
            };

            var ruleDataModel = new RuleDataModel
            {
                Content = content,
                ContentType = "ContentTypeSample",
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

            var contentSerializationProvider = Mock.Of<IContentSerializationProvider<ContentType>>();

            var ruleFactory = new RuleFactory<ContentType, ConditionType>(contentSerializationProvider);

            // Act
            var rule = ruleFactory.CreateRule(ruleDataModel);

            // Assert
            rule.Should().NotBeNull();
            rule.ContentContainer.Should().NotBeNull().And.BeOfType<SerializedContentContainer<ContentType>>();
            rule.DateBegin.Should().Be(ruleDataModel.DateBegin);
            rule.DateEnd.Should().BeNull();
            rule.Name.Should().Be(ruleDataModel.Name);
            rule.Priority.Should().Be(ruleDataModel.Priority);
            rule.RootCondition.Should().BeOfType<ComposedConditionNode<ConditionType>>();

            var composedConditionNode = rule.RootCondition.As<ComposedConditionNode<ConditionType>>();
            composedConditionNode.LogicalOperator.Should().Be(LogicalOperators.And);
            composedConditionNode.ChildConditionNodes.Should().HaveCount(4);

            var valueConditionNodes = composedConditionNode.ChildConditionNodes.OfType<ValueConditionNode<ConditionType>>();
            valueConditionNodes.Should().HaveCount(4);

            var integerConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.Integer);
            integerConditionNode.Should().NotBeNull();
            integerConditionNode.ConditionType.Should().Match(x => x == Enum.Parse<ConditionType>(integerConditionNodeDataModel.ConditionType));
            integerConditionNode.DataType.Should().Be(integerConditionNodeDataModel.DataType);
            integerConditionNode.LogicalOperator.Should().Be(integerConditionNodeDataModel.LogicalOperator);
            integerConditionNode.Operand.Should().Match(x => object.Equals(x, integerConditionNodeDataModel.Operand));
            integerConditionNode.Operator.Should().Be(integerConditionNodeDataModel.Operator);

            var stringConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.String);
            stringConditionNode.Should().NotBeNull();
            stringConditionNode.ConditionType.Should().Match(x => x == Enum.Parse<ConditionType>(stringConditionNodeDataModel.ConditionType));
            stringConditionNode.DataType.Should().Be(stringConditionNodeDataModel.DataType);
            stringConditionNode.LogicalOperator.Should().Be(stringConditionNodeDataModel.LogicalOperator);
            stringConditionNode.Operand.Should().Match(x => object.Equals(x, stringConditionNodeDataModel.Operand));
            stringConditionNode.Operator.Should().Be(stringConditionNodeDataModel.Operator);

            var decimalConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.Decimal);
            decimalConditionNode.Should().NotBeNull();
            decimalConditionNode.ConditionType.Should().Match(x => x == Enum.Parse<ConditionType>(decimalConditionNodeDataModel.ConditionType));
            decimalConditionNode.DataType.Should().Be(decimalConditionNodeDataModel.DataType);
            decimalConditionNode.LogicalOperator.Should().Be(decimalConditionNodeDataModel.LogicalOperator);
            decimalConditionNode.Operand.Should().Match(x => object.Equals(x, decimalConditionNodeDataModel.Operand));
            decimalConditionNode.Operator.Should().Be(decimalConditionNodeDataModel.Operator);

            var booleanConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.Boolean);
            booleanConditionNode.Should().NotBeNull();
            booleanConditionNode.ConditionType.Should().Match(x => x == Enum.Parse<ConditionType>(booleanConditionNodeDataModel.ConditionType));
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

            var contentSerializationProvider = Mock.Of<IContentSerializationProvider<ContentType>>();
            Mock.Get(contentSerializationProvider)
                .Setup(x => x.GetContentSerializer(ContentType.ContentTypeSample))
                .Returns(contentSerializer);

            var booleanConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionType.SampleBooleanCondition, Operators.NotEqual, true) as ValueConditionNode<ConditionType>;
            var decimalConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionType.SampleDecimalCondition, Operators.LesserThanOrEqual, 50.3m) as ValueConditionNode<ConditionType>;
            var integerConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionType.SampleIntegerCondition, Operators.GreaterThan, 20) as ValueConditionNode<ConditionType>;
            var stringConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionType.SampleStringCondition, Operators.Equal, "TEST") as ValueConditionNode<ConditionType>;

            var rule = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName("My rule used for testing purposes")
                .WithDateBegin(new DateTime(2020, 1, 1))
                .WithContent(ContentType.ContentTypeSample, (object)content)
                .WithCondition(c => c
                    .And(a => a
                        .Condition(booleanConditionNode)
                        .Condition(decimalConditionNode)
                        .Condition(integerConditionNode)
                        .Condition(stringConditionNode)
                    ))
                .Build().Rule;

            var ruleFactory = new RuleFactory<ContentType, ConditionType>(contentSerializationProvider);

            // Act
            var ruleDataModel = ruleFactory.CreateRule(rule);

            // Assert
            ruleDataModel.Should().NotBeNull();
            object ruleDataModelContent = ruleDataModel.Content;
            ruleDataModelContent.Should().NotBeNull().And.BeSameAs(content);
            ruleDataModel.DateBegin.Should().Be(ruleDataModel.DateBegin);
            ruleDataModel.DateEnd.Should().BeNull();
            ruleDataModel.Name.Should().Be(ruleDataModel.Name);
            ruleDataModel.Priority.Should().Be(ruleDataModel.Priority);
            ruleDataModel.RootCondition.Should().BeOfType<ComposedConditionNodeDataModel>();

            var composedConditionNodeDataModel = ruleDataModel.RootCondition.As<ComposedConditionNodeDataModel>();
            composedConditionNodeDataModel.LogicalOperator.Should().Be(LogicalOperators.And);
            composedConditionNodeDataModel.ChildConditionNodes.Should().HaveCount(4);

            var valueConditionNodeDataModels = composedConditionNodeDataModel.ChildConditionNodes.OfType<ValueConditionNodeDataModel>();
            valueConditionNodeDataModels.Should().HaveCount(4);

            var integerConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Integer);
            integerConditionNodeDataModel.Should().NotBeNull();
            integerConditionNodeDataModel.ConditionType.Should().Match<string>(x => integerConditionNode.ConditionType == Enum.Parse<ConditionType>(x));
            integerConditionNodeDataModel.DataType.Should().Be(integerConditionNode.DataType);
            integerConditionNodeDataModel.LogicalOperator.Should().Be(integerConditionNode.LogicalOperator);
            integerConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, integerConditionNode.Operand));
            integerConditionNodeDataModel.Operator.Should().Be(integerConditionNode.Operator);

            var stringConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.String);
            stringConditionNodeDataModel.Should().NotBeNull();
            stringConditionNodeDataModel.ConditionType.Should().Match<string>(x => stringConditionNode.ConditionType == Enum.Parse<ConditionType>(x));
            stringConditionNodeDataModel.DataType.Should().Be(stringConditionNode.DataType);
            stringConditionNodeDataModel.LogicalOperator.Should().Be(stringConditionNode.LogicalOperator);
            stringConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, stringConditionNode.Operand));
            stringConditionNodeDataModel.Operator.Should().Be(stringConditionNode.Operator);

            var decimalConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Decimal);
            decimalConditionNodeDataModel.Should().NotBeNull();
            decimalConditionNodeDataModel.ConditionType.Should().Match<string>(x => decimalConditionNode.ConditionType == Enum.Parse<ConditionType>(x));
            decimalConditionNodeDataModel.DataType.Should().Be(decimalConditionNode.DataType);
            decimalConditionNodeDataModel.LogicalOperator.Should().Be(decimalConditionNode.LogicalOperator);
            decimalConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, decimalConditionNode.Operand));
            decimalConditionNodeDataModel.Operator.Should().Be(decimalConditionNode.Operator);

            var booleanConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Boolean);
            booleanConditionNodeDataModel.Should().NotBeNull();
            booleanConditionNodeDataModel.ConditionType.Should().Match<string>(x => booleanConditionNode.ConditionType == Enum.Parse<ConditionType>(x));
            booleanConditionNodeDataModel.DataType.Should().Be(booleanConditionNode.DataType);
            booleanConditionNodeDataModel.LogicalOperator.Should().Be(booleanConditionNode.LogicalOperator);
            booleanConditionNodeDataModel.Operand.Should().Be(Convert.ToBoolean(booleanConditionNode.Operand));
            booleanConditionNodeDataModel.Operator.Should().Be(booleanConditionNode.Operator);
        }
    }
}