namespace Rules.Framework.Tests.Core.ConditionNodes
{
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class BooleanConditionNodeTests
    {
        [Fact]
        public void Init_GivenSetupWithBooleanValue_ReturnsSettedValues()
        {
            // Arrange
            var expectedConditionType = ConditionType.IsoCountryCode;
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = false;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Boolean;

            var sut = new ValueConditionNode<ConditionType>(DataTypes.Boolean, expectedConditionType, expectedOperator, expectedOperand);

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