namespace Rules.Framework.Tests.Core.ConditionNodes
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Tests.TestStubs;

    [TestClass]
    public class StringConditionNodeTests
    {
        [TestMethod]
        public void StringConditionNode_Init_GivenSetupWithStringValue_ReturnsSettedValues()
        {
            // Arrange
            ConditionType expectedConditionType = ConditionType.IsoCountryCode;
            Operators expectedOperator = Operators.NotEqual;
            string expectedOperand = "Such operand, much wow.";
            LogicalOperators expectedLogicalOperator = LogicalOperators.Eval;
            DataTypes expectedDataType = DataTypes.String;

            StringConditionNode<ConditionType> sut = new StringConditionNode<ConditionType>(expectedConditionType, expectedOperator, expectedOperand);

            // Act
            ConditionType actualConditionType = sut.ConditionType;
            Operators actualOperator = sut.Operator;
            DataTypes actualDataType = sut.DataType;
            LogicalOperators actualLogicalOperator = sut.LogicalOperator;
            string actualOperand = sut.Operand;

            // Assert
            Assert.AreEqual(expectedConditionType, actualConditionType);
            Assert.AreEqual(expectedOperator, actualOperator);
            Assert.AreEqual(expectedOperand, actualOperand);
            Assert.AreEqual(expectedLogicalOperator, actualLogicalOperator);
            Assert.AreEqual(expectedDataType, actualDataType);
        }
    }
}