namespace Rules.Framework.Tests.Core.ConditionNodes
{
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class IntegerConditionNodeTests
    {
        [Fact]
        public void Clone_NoConditions_ReturnsCloneInstance()
        {
            // Arrange
            var expectedConditionType = ConditionType.IsoCountryCode;
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = 1616;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Integer;

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
            valueConditionNode.Properties.Should().BeEmpty();
        }

        [Fact]
        public void Init_GivenSetupWithIntegerValue_ReturnsSettedValues()
        {
            // Arrange
            var expectedConditionType = ConditionType.IsoCountryCode;
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = 1616;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Integer;

            var sut = new ValueConditionNode<ConditionType>(expectedDataType, expectedConditionType, expectedOperator, expectedOperand);

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
    }
}