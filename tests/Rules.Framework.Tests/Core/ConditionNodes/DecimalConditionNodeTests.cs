namespace Rules.Framework.Tests.Core.ConditionNodes
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Tests.TestStubs;

    [TestClass]
    public class DecimalConditionNodeTests
    {
        [TestMethod]
        public void DecimalConditionNode_Init_GivenSetupWithDecimalValue_ReturnsSettedValues()
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
            Assert.AreEqual(expectedConditionType, actualConditionType);
            Assert.AreEqual(expectedOperator, actualOperator);
            Assert.AreEqual(expectedOperand, actualOperand);
            Assert.AreEqual(expectedLogicalOperator, actualLogicalOperator);
            Assert.AreEqual(expectedDataType, actualDataType);
        }
    }
}