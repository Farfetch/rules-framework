namespace Rules.Framework.Tests.Core.ConditionNodes
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Tests.TestStubs;

    [TestClass]
    public class IntegerConditionNodeTests
    {
        [TestMethod]
        public void IntegerConditionNode_Init_GivenSetupWithIntegerValue_ReturnsSettedValues()
        {
            // Arrange
            ConditionType expectedConditionType = ConditionType.IsoCountryCode;
            Operators expectedOperator = Operators.NotEqual;
            int expectedOperand = 1616;
            LogicalOperators expectedLogicalOperator = LogicalOperators.Eval;
            DataTypes expectedDataType = DataTypes.Integer;

            IntegerConditionNode<ConditionType> sut = new IntegerConditionNode<ConditionType>(expectedConditionType, expectedOperator, expectedOperand);

            // Act
            ConditionType actualConditionType = sut.ConditionType;
            Operators actualOperator = sut.Operator;
            DataTypes actualDataType = sut.DataType;
            LogicalOperators actualLogicalOperator = sut.LogicalOperator;
            int actualOperand = sut.Operand;

            // Assert
            Assert.AreEqual(expectedConditionType, actualConditionType);
            Assert.AreEqual(expectedOperator, actualOperator);
            Assert.AreEqual(expectedOperand, actualOperand);
            Assert.AreEqual(expectedLogicalOperator, actualLogicalOperator);
            Assert.AreEqual(expectedDataType, actualDataType);
        }
    }
}