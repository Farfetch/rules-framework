namespace Rules.Framework.Tests
{
    using FluentAssertions;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class ConditionTests
    {
        [Fact]
        public void Condition_Ctor_Success()
        {
            // Arrange
            var expectedType = ConditionNames.IsoCountryCode;
            var expectedValue = "abc";

            // Act
            var sut = new Condition<ConditionNames>(expectedType, expectedValue);

            // Assert
            sut.Type.Should().Be(expectedType);
            sut.Value.Should().Be(expectedValue);
        }
    }
}