namespace Rules.Framework.Tests.Builder
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;

    [TestClass]
    public class RuleEngineOptionsValidatorTests
    {
        [TestMethod]
        public void RuleEngineOptionsValidator_EnsureValid_GivenOptionsNullReference_ThrowsInvalidRulesEngineOptionsExceptionClaimingNullOptions()
        {
            // Arrange
            RulesEngineOptions rulesEngineOptions = null;

            RulesEngineOptionsValidator sut = new RulesEngineOptionsValidator();

            InvalidRulesEngineOptionsException actual = Assert.ThrowsException<InvalidRulesEngineOptionsException>(() =>
            {
                // Act
                sut.EnsureValid(rulesEngineOptions);
            });

            actual.Message.Should().Be("Specified null rulesEngineOptions.");
        }


        [TestMethod]
        public void RuleEngineOptionsValidator_EnsureValid_GivenOptionsWithInvalidDefaultForBooleanDataType_ThrowsInvalidRulesEngineOptionsExceptionClaimingInvalidDefault()
        {
            // Arrange
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.DataTypeDefaults[DataTypes.Boolean] = "abc";

            RulesEngineOptionsValidator sut = new RulesEngineOptionsValidator();

            InvalidRulesEngineOptionsException actual = Assert.ThrowsException<InvalidRulesEngineOptionsException>(() =>
            {
                // Act
                sut.EnsureValid(rulesEngineOptions);
            });

            actual.Message.Should().Be("Specified invalid default value for data type Boolean: abc.");
        }


        [TestMethod]
        public void RuleEngineOptionsValidator_EnsureValid_GivenOptionsWithNullDefaultForBooleanDataType_ThrowsInvalidRulesEngineOptionsExceptionClaimingInvalidDefault()
        {
            // Arrange
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.DataTypeDefaults[DataTypes.Boolean] = null;

            RulesEngineOptionsValidator sut = new RulesEngineOptionsValidator();

            InvalidRulesEngineOptionsException actual = Assert.ThrowsException<InvalidRulesEngineOptionsException>(() =>
            {
                // Act
                sut.EnsureValid(rulesEngineOptions);
            });

            actual.Message.Should().Be("Specified invalid default value for data type Boolean: null.");
        }


        [TestMethod]
        public void RuleEngineOptionsValidator_EnsureValid_GivenOptionsWithInvalidDefaultForDecimalDataType_ThrowsInvalidRulesEngineOptionsExceptionClaimingInvalidDefault()
        {
            // Arrange
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.DataTypeDefaults[DataTypes.Decimal] = "abc";

            RulesEngineOptionsValidator sut = new RulesEngineOptionsValidator();

            InvalidRulesEngineOptionsException actual = Assert.ThrowsException<InvalidRulesEngineOptionsException>(() =>
            {
                // Act
                sut.EnsureValid(rulesEngineOptions);
            });

            actual.Message.Should().Be("Specified invalid default value for data type Decimal: abc.");
        }


        [TestMethod]
        public void RuleEngineOptionsValidator_EnsureValid_GivenOptionsWithNullDefaultForDecimalDataType_ThrowsInvalidRulesEngineOptionsExceptionClaimingInvalidDefault()
        {
            // Arrange
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.DataTypeDefaults[DataTypes.Decimal] = null;

            RulesEngineOptionsValidator sut = new RulesEngineOptionsValidator();

            InvalidRulesEngineOptionsException actual = Assert.ThrowsException<InvalidRulesEngineOptionsException>(() =>
            {
                // Act
                sut.EnsureValid(rulesEngineOptions);
            });

            actual.Message.Should().Be("Specified invalid default value for data type Decimal: null.");
        }


        [TestMethod]
        public void RuleEngineOptionsValidator_EnsureValid_GivenOptionsWithInvalidDefaultForIntegerDataType_ThrowsInvalidRulesEngineOptionsExceptionClaimingInvalidDefault()
        {
            // Arrange
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.DataTypeDefaults[DataTypes.Integer] = "abc";

            RulesEngineOptionsValidator sut = new RulesEngineOptionsValidator();

            InvalidRulesEngineOptionsException actual = Assert.ThrowsException<InvalidRulesEngineOptionsException>(() =>
            {
                // Act
                sut.EnsureValid(rulesEngineOptions);
            });

            actual.Message.Should().Be("Specified invalid default value for data type Integer: abc.");
        }


        [TestMethod]
        public void RuleEngineOptionsValidator_EnsureValid_GivenOptionsWithNullDefaultForIntegerDataType_ThrowsInvalidRulesEngineOptionsExceptionClaimingInvalidDefault()
        {
            // Arrange
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.DataTypeDefaults[DataTypes.Integer] = null;

            RulesEngineOptionsValidator sut = new RulesEngineOptionsValidator();

            InvalidRulesEngineOptionsException actual = Assert.ThrowsException<InvalidRulesEngineOptionsException>(() =>
            {
                // Act
                sut.EnsureValid(rulesEngineOptions);
            });

            actual.Message.Should().Be("Specified invalid default value for data type Integer: null.");
        }

        [TestMethod]
        public void RuleEngineOptionsValidator_EnsureValid_GivenOptionsWithInvalidDefaultForStringDataType_ThrowsInvalidRulesEngineOptionsExceptionClaimingInvalidDefault()
        {
            // Arrange
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.DataTypeDefaults[DataTypes.String] = 0;

            RulesEngineOptionsValidator sut = new RulesEngineOptionsValidator();

            InvalidRulesEngineOptionsException actual = Assert.ThrowsException<InvalidRulesEngineOptionsException>(() =>
            {
                // Act
                sut.EnsureValid(rulesEngineOptions);
            });

            actual.Message.Should().Be("Specified invalid default value for data type String: 0.");
        }

        [TestMethod]
        public void RuleEngineOptionsValidator_EnsureValid_GivenOptionsWithNullDefaultForStringDataType_ThrowsInvalidRulesEngineOptionsExceptionClaimingInvalidDefault()
        {
            // Arrange
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.DataTypeDefaults[DataTypes.String] = null;

            RulesEngineOptionsValidator sut = new RulesEngineOptionsValidator();

            InvalidRulesEngineOptionsException actual = Assert.ThrowsException<InvalidRulesEngineOptionsException>(() =>
            {
                // Act
                sut.EnsureValid(rulesEngineOptions);
            });

            actual.Message.Should().Be("Specified invalid default value for data type String: null.");
        }
    }
}
