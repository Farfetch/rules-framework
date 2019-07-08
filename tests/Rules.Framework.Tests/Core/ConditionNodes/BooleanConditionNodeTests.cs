using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rules.Framework.Core;
using Rules.Framework.Core.ConditionNodes;
using Rules.Framework.Tests.TestStubs;

namespace Rules.Framework.Tests.Core.ConditionNodes
{
    [TestClass]
    public class BooleanConditionNodeTests
    {
        [TestMethod]
        public void BooleanConditionNode_Init_GivenSetupWithBooleanValue_ReturnsSettedValues()
        {
            // Arrange
            ConditionType expectedConditionType = ConditionType.IsoCountryCode;
            Operators expectedOperator = Operators.NotEqual;
            bool expectedOperand = false;
            LogicalOperators expectedLogicalOperator = LogicalOperators.Eval;
            DataTypes expectedDataType = DataTypes.Boolean;

            BooleanConditionNode<ConditionType> sut = new BooleanConditionNode<ConditionType>(expectedConditionType, expectedOperator, expectedOperand);

            // Act
            ConditionType actualConditionType = sut.ConditionType;
            Operators actualOperator = sut.Operator;
            DataTypes actualDataType = sut.DataType;
            LogicalOperators actualLogicalOperator = sut.LogicalOperator;
            bool actualOperand = sut.Operand;

            // Assert
            Assert.AreEqual(expectedConditionType, actualConditionType);
            Assert.AreEqual(expectedOperator, actualOperator);
            Assert.AreEqual(expectedOperand, actualOperand);
            Assert.AreEqual(expectedLogicalOperator, actualLogicalOperator);
            Assert.AreEqual(expectedDataType, actualDataType);
        }
    }
}