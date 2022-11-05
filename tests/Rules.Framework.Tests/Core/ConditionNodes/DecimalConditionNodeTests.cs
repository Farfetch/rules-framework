namespace Rules.Framework.Tests.Core.ConditionNodes
{
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class DecimalConditionNodeTests
    {
        [Fact]
        public void Init_GivenSetupWithDecimalValue_ReturnsSettedValues()
        {
            // Arrange
            ConditionType expectedConditionType = ConditionType.PluviosityRate;
            Operators expectedOperator = Operators.NotEqual;
            decimal expectedOperand = 5682.2654m;
            LogicalOperators expectedLogicalOperator = LogicalOperators.Eval;
            DataTypes expectedDataType = DataTypes.Decimal;

            DecimalConditionNode<ConditionType> sut = new DecimalConditionNode<ConditionType>(expectedConditionType, expectedOperator, expectedOperand);

            // Act
            ConditionType actualConditionType = sut.ConditionType;
            Operators actualOperator = sut.Operator;
            DataTypes actualDataType = sut.DataType;
            LogicalOperators actualLogicalOperator = sut.LogicalOperator;
            decimal actualOperand = sut.Operand;

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