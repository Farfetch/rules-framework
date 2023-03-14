namespace Rules.Framework.Tests
{
    using FluentAssertions;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class ConditionTests
    {
        [Fact]
        public void Type_HavingSettedType_ReturnsSettedValue()
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
            actual.Should().Be(expected);
        }

        [Fact]
        public void Value_HavingSettedValue_ReturnsSettedValue()
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
            actual.Should().Be(expected);
        }
    }
}