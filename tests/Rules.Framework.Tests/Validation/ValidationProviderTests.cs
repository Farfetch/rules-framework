namespace Rules.Framework.Tests.Validation
{
    using System;
    using FluentAssertions;
    using FluentValidation;
    using Moq;
    using Rules.Framework.Validation;
    using Xunit;

    public class ValidationProviderTests
    {
        [Fact]
        public void GetValidatorFor_GivenMappedType_ReturnsValidator()
        {
            // Arrange
            IValidator<object> expectedValidator = Mock.Of<IValidator<object>>();

            ValidationProvider validationProvider = ValidationProvider.New()
                .MapValidatorFor(expectedValidator);

            // Act
            IValidator actualValidator = validationProvider.GetValidatorFor<object>();

            // Assert
            actualValidator.Should().NotBeNull();
            actualValidator.Should().BeSameAs(expectedValidator);
        }

        [Fact]
        public void GetValidatorFor_GivenUnmappedType_ThrowsNotSupportedException()
        {
            // Arrange
            ValidationProvider validationProvider = ValidationProvider.New();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => validationProvider.GetValidatorFor<object>());

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(typeof(object).Name);
        }
    }
}