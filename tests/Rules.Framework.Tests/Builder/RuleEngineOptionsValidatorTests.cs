namespace Rules.Framework.Tests.Builder
{
    using FluentAssertions;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Xunit;

    public class RuleEngineOptionsValidatorTests
    {
        [Fact]
        public void EnsureValid_GivenOptionsNullReference_ThrowsInvalidRulesEngineOptionsExceptionClaimingNullOptions()
        {
            // Arrange
            RulesEngineOptions rulesEngineOptions = null;

            InvalidRulesEngineOptionsException actual = Assert.Throws<InvalidRulesEngineOptionsException>(() =>
            {
                // Act
                RulesEngineOptionsValidator.EnsureValid(rulesEngineOptions);
            });

            actual.Message.Should().Be("Specified null rulesEngineOptions.");
        }

        [Theory]
        [InlineData(DataTypes.Boolean, "abc")]
        [InlineData(DataTypes.Boolean, null)]
        [InlineData(DataTypes.Decimal, "abc")]
        [InlineData(DataTypes.Decimal, null)]
        [InlineData(DataTypes.Integer, "abc")]
        [InlineData(DataTypes.Integer, null)]
        [InlineData(DataTypes.String, 0)]
        [InlineData(DataTypes.String, null)]
        public void EnsureValid_GivenOptionsWithInvalidDefaultForDataType_ThrowsInvalidRulesEngineOptionsExceptionClaimingInvalidDefault(DataTypes dataType, object defaultValue)
        {
            // Arrange
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.DataTypeDefaults[dataType] = defaultValue;

            InvalidRulesEngineOptionsException actual = Assert.Throws<InvalidRulesEngineOptionsException>(() =>
            {
                // Act
                RulesEngineOptionsValidator.EnsureValid(rulesEngineOptions);
            });

            actual.Message.Should().Be($"Specified invalid default value for data type {dataType}: {defaultValue ?? "null"}.");
        }
    }
}