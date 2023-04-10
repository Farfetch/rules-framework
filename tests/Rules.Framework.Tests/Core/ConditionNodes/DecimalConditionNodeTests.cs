namespace Rules.Framework.Tests.Core.ConditionNodes
{
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class DecimalConditionNodeTests
    {
        [Fact]
        public void Clone_NoConditions_ReturnsCloneInstance()
        {
            // Arrange
            var expectedConditionType = ConditionType.PluviosityRate;
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = 5682.2654m;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Decimal;

            var sut = new ValueConditionNode<ConditionType>(expectedDataType, expectedConditionType, expectedOperator, expectedOperand);
            sut.Properties["test"] = "test";

            // Act
            var actual = sut.Clone();

            // Assert
            actual.Should()
                .NotBeNull()
                .And
                .BeOfType<ValueConditionNode<ConditionType>>();
            var valueConditionNode = actual.As<ValueConditionNode<ConditionType>>();
            valueConditionNode.ConditionType.Should().Be(expectedConditionType);
            valueConditionNode.DataType.Should().Be(expectedDataType);
            valueConditionNode.LogicalOperator.Should().Be(expectedLogicalOperator);
            valueConditionNode.Operator.Should().Be(expectedOperator);
            valueConditionNode.Operand.Should().Be(expectedOperand);
            valueConditionNode.Properties.Should().BeEquivalentTo(sut.Properties);
        }

        [Fact]
        public void Init_GivenSetupWithDecimalValue_ReturnsSettedValues()
        {
            // Arrange
            var expectedConditionType = ConditionType.PluviosityRate;
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = 5682.2654m;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Decimal;

            var sut = new ValueConditionNode<ConditionType>(DataTypes.Decimal, expectedConditionType, expectedOperator, expectedOperand);

            // Act
            var actualConditionType = sut.ConditionType;
            var actualOperator = sut.Operator;
            var actualDataType = sut.DataType;
            var actualLogicalOperator = sut.LogicalOperator;
            var actualOperand = sut.Operand;

            // Assert
            actualConditionType.Should().Be(expectedConditionType);
            actualOperator.Should().Be(expectedOperator);
            actualOperand.Should().Be(expectedOperand);
            actualLogicalOperator.Should().Be(expectedLogicalOperator);
            actualDataType.Should().Be(expectedDataType);
        }

        [Fact]
        public void Clone_NoConditions_ReturnsCloneInstance()
        {
            // Arrange
            ConditionType expectedConditionType = ConditionType.PluviosityRate;
            Operators expectedOperator = Operators.NotEqual;
            decimal expectedOperand = 5682.2654m;
            LogicalOperators expectedLogicalOperator = LogicalOperators.Eval;
            DataTypes expectedDataType = DataTypes.Decimal;

            DecimalConditionNode<ConditionType> sut = new DecimalConditionNode<ConditionType>(expectedConditionType, expectedOperator, expectedOperand);
            sut.Properties["test"] = "test";

            // Act
            IConditionNode<ConditionType> actual = sut.Clone();

            // Assert
            actual.Should()
                .NotBeNull()
                .And
                .BeOfType<DecimalConditionNode<ConditionType>>();
            DecimalConditionNode<ConditionType> valueConditionNode = actual.As<DecimalConditionNode<ConditionType>>();
            valueConditionNode.ConditionType.Should().Be(expectedConditionType);
            valueConditionNode.DataType.Should().Be(expectedDataType);
            valueConditionNode.LogicalOperator.Should().Be(expectedLogicalOperator);
            valueConditionNode.Operator.Should().Be(expectedOperator);
            valueConditionNode.Operand.Should().Be(expectedOperand);
            valueConditionNode.Properties.Should().BeEmpty();
        }
    }
}