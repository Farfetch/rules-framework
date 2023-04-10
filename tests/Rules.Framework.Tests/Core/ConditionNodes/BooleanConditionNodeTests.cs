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
        public void Clone_NoConditions_ReturnsCloneInstance()
        {
            // Arrange
            var expectedConditionType = ConditionType.IsoCountryCode;
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = false;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Boolean;

            var sut = new ValueConditionNode<ConditionType>(DataTypes.Boolean, expectedConditionType, expectedOperator, expectedOperand);
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
        public void Init_GivenSetupWithBooleanValue_ReturnsSettedValues()
        {
            // Arrange
            var expectedConditionType = ConditionType.IsoCountryCode;
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = false;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Boolean;

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

        [Fact]
        public void Clone_NoConditions_ReturnsCloneInstance()
        {
            // Arrange
            ConditionType expectedConditionType = ConditionType.IsoCountryCode;
            Operators expectedOperator = Operators.NotEqual;
            bool expectedOperand = false;
            LogicalOperators expectedLogicalOperator = LogicalOperators.Eval;
            DataTypes expectedDataType = DataTypes.Boolean;

            BooleanConditionNode<ConditionType> sut = new BooleanConditionNode<ConditionType>(expectedConditionType, expectedOperator, expectedOperand);
            sut.Properties["test"] = "test";

            // Act
            IConditionNode<ConditionType> actual = sut.Clone();

            // Assert
            actual.Should()
                .NotBeNull()
                .And
                .BeOfType<BooleanConditionNode<ConditionType>>();
            BooleanConditionNode<ConditionType> valueConditionNode = actual.As<BooleanConditionNode<ConditionType>>();
            valueConditionNode.ConditionType.Should().Be(expectedConditionType);
            valueConditionNode.DataType.Should().Be(expectedDataType);
            valueConditionNode.LogicalOperator.Should().Be(expectedLogicalOperator);
            valueConditionNode.Operator.Should().Be(expectedOperator);
            valueConditionNode.Operand.Should().Be(expectedOperand);
            valueConditionNode.Properties.Should().BeEmpty();
        }
    }
}