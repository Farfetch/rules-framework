using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rules.Framework.Tests.TestStubs;

namespace Rules.Framework.Tests
{
    [TestClass]
    public class ConditionTests
    {
        [TestMethod]
        public void Condition_Type_HavingSettedType_ReturnsSettedValue()
        {
            // Arrange
            ConditionType expected = ConditionType.IsoCountryCode;

            Condition<ConditionType> sut = new Condition<ConditionType>
            {
                Type = expected
            };

            // Act
            ConditionType actual = sut.Type;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Condition_Value_HavingSettedValue_ReturnsSettedValue()
        {
            // Arrange
            object expected = "abc";

            Condition<ConditionType> sut = new Condition<ConditionType>
            {
                Value = expected
            };

            // Act
            object actual = sut.Value;

            // Assert
            Assert.AreSame(expected, actual);
        }
    }
}