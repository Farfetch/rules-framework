namespace Rules.Framework.Tests.ConditionNodes
{
    using FluentAssertions;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class ValueConditionNodeTests
    {
        [Fact]
        public void Clone_BooleanDataType_ReturnsCloneInstance()
        {
            // Arrange
            var expectedCondition = ConditionNames.IsoCountryCode.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = false;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Boolean;

            var sut = new ValueConditionNode(DataTypes.Boolean, expectedCondition, expectedOperator, expectedOperand);
            sut.Properties["test"] = "test";

            // Act
            var actual = sut.Clone();

            // Assert
            actual.Should()
                .NotBeNull()
                .And
                .BeOfType<ValueConditionNode>();
            var valueConditionNode = actual.As<ValueConditionNode>();
            valueConditionNode.Condition.Should().Be(expectedCondition);
            valueConditionNode.DataType.Should().Be(expectedDataType);
            valueConditionNode.LogicalOperator.Should().Be(expectedLogicalOperator);
            valueConditionNode.Operator.Should().Be(expectedOperator);
            valueConditionNode.Operand.Should().Be(expectedOperand);
            valueConditionNode.Properties.Should().BeEquivalentTo(sut.Properties);
        }

        [Fact]
        public void Clone_DecimalDataType_ReturnsCloneInstance()
        {
            // Arrange
            var expectedCondition = ConditionNames.PluviosityRate.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = 5682.2654m;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Decimal;

            var sut = new ValueConditionNode(expectedDataType, expectedCondition, expectedOperator, expectedOperand);
            sut.Properties["test"] = "test";

            // Act
            var actual = sut.Clone();

            // Assert
            actual.Should()
                .NotBeNull()
                .And
                .BeOfType<ValueConditionNode>();
            var valueConditionNode = actual.As<ValueConditionNode>();
            valueConditionNode.Condition.Should().Be(expectedCondition);
            valueConditionNode.DataType.Should().Be(expectedDataType);
            valueConditionNode.LogicalOperator.Should().Be(expectedLogicalOperator);
            valueConditionNode.Operator.Should().Be(expectedOperator);
            valueConditionNode.Operand.Should().Be(expectedOperand);
            valueConditionNode.Properties.Should().BeEquivalentTo(sut.Properties);
        }

        [Fact]
        public void Clone_IntegerDataType_ReturnsCloneInstance()
        {
            // Arrange
            var expectedCondition = ConditionNames.IsoCountryCode.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = 1616;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Integer;

            var sut = new ValueConditionNode(expectedDataType, expectedCondition, expectedOperator, expectedOperand);
            sut.Properties["test"] = "test";

            // Act
            var actual = sut.Clone();

            // Assert
            actual.Should()
                .NotBeNull()
                .And
                .BeOfType<ValueConditionNode>();
            var valueConditionNode = actual.As<ValueConditionNode>();
            valueConditionNode.Condition.Should().Be(expectedCondition);
            valueConditionNode.DataType.Should().Be(expectedDataType);
            valueConditionNode.LogicalOperator.Should().Be(expectedLogicalOperator);
            valueConditionNode.Operator.Should().Be(expectedOperator);
            valueConditionNode.Operand.Should().Be(expectedOperand);
            valueConditionNode.Properties.Should().BeEquivalentTo(sut.Properties);
        }

        [Fact]
        public void Clone_StringDataType_ReturnsCloneInstance()
        {
            // Arrange
            var expectedCondition = ConditionNames.IsoCountryCode.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = "Such operand, much wow.";
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.String;

            var sut = new ValueConditionNode(expectedDataType, expectedCondition, expectedOperator, expectedOperand);
            sut.Properties["test"] = "test";

            // Act
            var actual = sut.Clone();

            // Assert
            actual.Should()
                .NotBeNull()
                .And
                .BeOfType<ValueConditionNode>();
            var valueConditionNode = actual.As<ValueConditionNode>();
            valueConditionNode.Condition.Should().Be(expectedCondition);
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
            var expectedCondition = ConditionNames.IsoCountryCode.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = false;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Boolean;

            var sut = new ValueConditionNode(expectedDataType, expectedCondition, expectedOperator, expectedOperand);

            // Act
            var actualCondition = sut.Condition;
            var actualOperator = sut.Operator;
            var actualDataType = sut.DataType;
            var actualLogicalOperator = sut.LogicalOperator;
            var actualOperand = sut.Operand;

            // Assert
            actualCondition.Should().Be(expectedCondition);
            actualOperator.Should().Be(expectedOperator);
            actualOperand.Should().Be(expectedOperand);
            actualLogicalOperator.Should().Be(expectedLogicalOperator);
            actualDataType.Should().Be(expectedDataType);
        }

        [Fact]
        public void Init_GivenSetupWithDecimalValue_ReturnsSettedValues()
        {
            // Arrange
            var expectedCondition = ConditionNames.PluviosityRate.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = 5682.2654m;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Decimal;

            var sut = new ValueConditionNode(DataTypes.Decimal, expectedCondition, expectedOperator, expectedOperand);

            // Act
            var actualCondition = sut.Condition;
            var actualOperator = sut.Operator;
            var actualDataType = sut.DataType;
            var actualLogicalOperator = sut.LogicalOperator;
            var actualOperand = sut.Operand;

            // Assert
            actualCondition.Should().Be(expectedCondition);
            actualOperator.Should().Be(expectedOperator);
            actualOperand.Should().Be(expectedOperand);
            actualLogicalOperator.Should().Be(expectedLogicalOperator);
            actualDataType.Should().Be(expectedDataType);
        }

        [Fact]
        public void Init_GivenSetupWithIntegerValue_ReturnsSettedValues()
        {
            // Arrange
            var expectedCondition = ConditionNames.IsoCountryCode.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = 1616;
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.Integer;

            var sut = new ValueConditionNode(expectedDataType, expectedCondition, expectedOperator, expectedOperand);

            // Act
            var actualCondition = sut.Condition;
            var actualOperator = sut.Operator;
            var actualDataType = sut.DataType;
            var actualLogicalOperator = sut.LogicalOperator;
            var actualOperand = sut.Operand;

            // Assert
            actualCondition.Should().Be(expectedCondition);
            actualOperator.Should().Be(expectedOperator);
            actualOperand.Should().Be(expectedOperand);
            actualLogicalOperator.Should().Be(expectedLogicalOperator);
            actualDataType.Should().Be(expectedDataType);
        }

        [Fact]
        public void Init_GivenSetupWithStringValue_ReturnsSettedValues()
        {
            // Arrange
            var expectedCondition = ConditionNames.IsoCountryCode.ToString();
            var expectedOperator = Operators.NotEqual;
            var expectedOperand = "Such operand, much wow.";
            var expectedLogicalOperator = LogicalOperators.Eval;
            var expectedDataType = DataTypes.String;

            var sut = new ValueConditionNode(expectedDataType, expectedCondition, expectedOperator, expectedOperand);

            // Act
            var actualCondition = sut.Condition;
            var actualOperator = sut.Operator;
            var actualDataType = sut.DataType;
            var actualLogicalOperator = sut.LogicalOperator;
            var actualOperand = sut.Operand;

            // Assert
            actualCondition.Should().Be(expectedCondition);
            actualOperator.Should().Be(expectedOperator);
            actualOperand.Should().Be(expectedOperand);
            actualLogicalOperator.Should().Be(expectedLogicalOperator);
            actualDataType.Should().Be(expectedDataType);
        }
    }
}