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
        public void Init_GivenSetupWithDecimalValue_ReturnsSettedValues()
        {
            // Arrange
            var expectedConditionType = ConditionType.PluviosityRate;
            var expectedOperator = Operators.NotEqual;
            decimal expectedOperand = 5682.2654m;
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
    }
}