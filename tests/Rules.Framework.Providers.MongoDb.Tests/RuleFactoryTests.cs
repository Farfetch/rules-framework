namespace Rules.Framework.Providers.MongoDb.Tests
{
    using System;
    using System.Collections.Generic;
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

            IContentSerializationProvider<ContentType> contentSerializationProvider = Mock.Of<IContentSerializationProvider<ContentType>>();

            RuleFactory<ContentType, ConditionType> ruleFactory = new RuleFactory<ContentType, ConditionType>(contentSerializationProvider);

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

            IEnumerable<ValueConditionNode<ConditionType>> valueConditionNodes = composedConditionNode.ChildConditionNodes.OfType<ValueConditionNode<ConditionType>>();
            valueConditionNodes.Should().HaveCount(4);
            ValueConditionNode<ConditionType> integerConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.Integer);
            integerConditionNode.Should().NotBeNull();
            integerConditionNode.ConditionType.Should().Match(x => x == Enum.Parse<ConditionType>(integerConditionNodeDataModel.ConditionType));
            integerConditionNode.DataType.Should().Be(integerConditionNodeDataModel.DataType);
            integerConditionNode.LogicalOperator.Should().Be(integerConditionNodeDataModel.LogicalOperator);
            integerConditionNode.Operand.Should().Match(x => object.Equals(x, integerConditionNodeDataModel.Operand));
            integerConditionNode.Operator.Should().Be(integerConditionNodeDataModel.Operator);

            ValueConditionNode<ConditionType> stringConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.String);
            stringConditionNode.Should().NotBeNull();
            stringConditionNode.ConditionType.Should().Match(x => x == Enum.Parse<ConditionType>(stringConditionNodeDataModel.ConditionType));
            stringConditionNode.DataType.Should().Be(stringConditionNodeDataModel.DataType);
            stringConditionNode.LogicalOperator.Should().Be(stringConditionNodeDataModel.LogicalOperator);
            stringConditionNode.Operand.Should().Match(x => object.Equals(x, stringConditionNodeDataModel.Operand));
            stringConditionNode.Operator.Should().Be(stringConditionNodeDataModel.Operator);

            ValueConditionNode<ConditionType> decimalConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.Decimal);
            decimalConditionNode.Should().NotBeNull();
            decimalConditionNode.ConditionType.Should().Match(x => x == Enum.Parse<ConditionType>(decimalConditionNodeDataModel.ConditionType));
            decimalConditionNode.DataType.Should().Be(decimalConditionNodeDataModel.DataType);
            decimalConditionNode.LogicalOperator.Should().Be(decimalConditionNodeDataModel.LogicalOperator);
            decimalConditionNode.Operand.Should().Match(x => object.Equals(x, decimalConditionNodeDataModel.Operand));
            decimalConditionNode.Operator.Should().Be(decimalConditionNodeDataModel.Operator);

            ValueConditionNode<ConditionType> booleanConditionNode = valueConditionNodes.First(x => x.DataType == DataTypes.Boolean);
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

            IContentSerializer contentSerializer = Mock.Of<IContentSerializer>();
            Mock.Get(contentSerializer)
                .Setup(x => x.Deserialize(It.IsAny<object>(), It.IsAny<Type>()))
                .Returns((object)content);

            IContentSerializationProvider<ContentType> contentSerializationProvider = Mock.Of<IContentSerializationProvider<ContentType>>();
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

            Rule<ContentType, ConditionType> rule1 = RuleBuilder.NewRule<ContentType, ConditionType>()
                .WithName("My rule used for testing purposes")
                .WithDateBegin(new DateTime(2020, 1, 1))
                .WithContent(ContentType.ContentTypeSample, (object)content)
                .WithCondition(c => c
                    .And(a => a
                        .Value(booleanConditionNode)
                        .Value(decimalConditionNode)
                        .Value(integerConditionNode)
                        .Value(stringConditionNode)
                    ))
                .Build().Rule;

            RuleFactory<ContentType, ConditionType> ruleFactory = new RuleFactory<ContentType, ConditionType>(contentSerializationProvider);

            // Act
            RuleDataModel rule = ruleFactory.CreateRule(rule1);

            // Assert
            rule.Should().NotBeNull();
            object content1 = rule.Content;
            content1.Should().NotBeNull()
                .And.BeSameAs(content);
            rule.DateBegin.Should().Be(rule.DateBegin);
            rule.DateEnd.Should().BeNull();
            rule.Name.Should().Be(rule.Name);
            rule.Priority.Should().Be(rule.Priority);
            rule.RootCondition.Should().BeOfType<ComposedConditionNodeDataModel>();

            ComposedConditionNodeDataModel composedConditionNodeDataModel = rule.RootCondition.As<ComposedConditionNodeDataModel>();
            composedConditionNodeDataModel.LogicalOperator.Should().Be(LogicalOperators.And);
            composedConditionNodeDataModel.ChildConditionNodes.Should().HaveCount(4);

            IEnumerable<ValueConditionNodeDataModel> valueConditionNodeDataModels = composedConditionNodeDataModel.ChildConditionNodes.OfType<ValueConditionNodeDataModel>();
            valueConditionNodeDataModels.Should().HaveCount(4);
            ValueConditionNodeDataModel integerConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Integer);
            integerConditionNodeDataModel.Should().NotBeNull();
            integerConditionNodeDataModel.ConditionType.Should().Match<string>(x => integerConditionNode.ConditionType == Enum.Parse<ConditionType>(x));
            integerConditionNodeDataModel.DataType.Should().Be(integerConditionNode.DataType);
            integerConditionNodeDataModel.LogicalOperator.Should().Be(integerConditionNode.LogicalOperator);
            integerConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, integerConditionNode.Operand));
            integerConditionNodeDataModel.Operator.Should().Be(integerConditionNode.Operator);

            ValueConditionNodeDataModel stringConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.String);
            stringConditionNodeDataModel.Should().NotBeNull();
            stringConditionNodeDataModel.ConditionType.Should().Match<string>(x => stringConditionNode.ConditionType == Enum.Parse<ConditionType>(x));
            stringConditionNodeDataModel.DataType.Should().Be(stringConditionNode.DataType);
            stringConditionNodeDataModel.LogicalOperator.Should().Be(stringConditionNode.LogicalOperator);
            stringConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, stringConditionNode.Operand));
            stringConditionNodeDataModel.Operator.Should().Be(stringConditionNode.Operator);

            ValueConditionNodeDataModel decimalConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Decimal);
            decimalConditionNodeDataModel.Should().NotBeNull();
            decimalConditionNodeDataModel.ConditionType.Should().Match<string>(x => decimalConditionNode.ConditionType == Enum.Parse<ConditionType>(x));
            decimalConditionNodeDataModel.DataType.Should().Be(decimalConditionNode.DataType);
            decimalConditionNodeDataModel.LogicalOperator.Should().Be(decimalConditionNode.LogicalOperator);
            decimalConditionNodeDataModel.Operand.Should().Match(x => object.Equals(x, decimalConditionNode.Operand));
            decimalConditionNodeDataModel.Operator.Should().Be(decimalConditionNode.Operator);

            ValueConditionNodeDataModel booleanConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.DataType == DataTypes.Boolean);
            booleanConditionNodeDataModel.Should().NotBeNull();
            booleanConditionNodeDataModel.ConditionType.Should().Match<string>(x => booleanConditionNode.ConditionType == Enum.Parse<ConditionType>(x));
            booleanConditionNodeDataModel.DataType.Should().Be(booleanConditionNode.DataType);
            booleanConditionNodeDataModel.LogicalOperator.Should().Be(booleanConditionNode.LogicalOperator);
            booleanConditionNodeDataModel.Operand.Should().Be(Convert.ToBoolean(booleanConditionNode.Operand));
            booleanConditionNodeDataModel.Operator.Should().Be(booleanConditionNode.Operator);
        }
    }
}