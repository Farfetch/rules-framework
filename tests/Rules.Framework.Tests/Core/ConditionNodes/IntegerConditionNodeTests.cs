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
        public void Init_GivenSetupWithIntegerValue_ReturnsSettedValues()
        {
            // Arrange
            var expectedConditionType = ConditionType.IsoCountryCode;
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = 1616;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Integer;

            var sut = new ValueConditionNode<ConditionType>(DataTypes.Integer, expectedConditionType, expectedOperator, expectedOperand);

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