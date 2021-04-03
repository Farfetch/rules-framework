namespace Rules.Framework.Tests.Validation
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using FluentValidation.Results;
    using Rules.Framework.Tests.TestStubs;
    using Rules.Framework.Validation;
    using Xunit;

    public class SearchArgsValidatorTests
    {
        [Fact]
        public void Validate_GivenConditionWithTypeAsAsEnumTypeAndUndefinedValue_ReturnsFailedValidation()
        {
            // Arrange
            SearchArgs<ContentType, ConditionType> searchArgs = new SearchArgs<ContentType, ConditionType>(ContentType.Type1, DateTime.MinValue, DateTime.MaxValue)
            {
                Conditions = new[]
                {
                    new Condition<ConditionType>
                    {
                        Type = 0,
                        Value = 1
                    }
                }
            };

            SearchArgsValidator<ContentType, ConditionType> validator = new SearchArgsValidator<ContentType, ConditionType>();

            // Act
            ValidationResult validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().HaveCount(1);
            validationResult.Errors.Should().Match(c => c.Any(vf => vf.PropertyName == $"{nameof(searchArgs.Conditions)}[0].Type"));
        }

        [Fact]
        public void Validate_GivenConditionWithTypeAsClassTypeAndNotNullValue_ReturnsSuccessValidation()
        {
            // Arrange
            SearchArgs<ContentType, ConditionTypeClass> searchArgs = new SearchArgs<ContentType, ConditionTypeClass>(ContentType.Type1, DateTime.MinValue, DateTime.MaxValue)
            {
                Conditions = new[]
                {
                    new Condition<ConditionTypeClass>
                    {
                        Type = new ConditionTypeClass
                        {
                            Id = 1,
                            Name = "Sample Condition Type"
                        },
                        Value = 1
                    }
                }
            };

            SearchArgsValidator<ContentType, ConditionTypeClass> validator = new SearchArgsValidator<ContentType, ConditionTypeClass>();

            // Act
            ValidationResult validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeTrue();
            validationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_GivenConditionWithTypeAsClassTypeAndNullValue_ReturnsFailedValidation()
        {
            // Arrange
            SearchArgs<ContentType, ConditionTypeClass> searchArgs = new SearchArgs<ContentType, ConditionTypeClass>(ContentType.Type1, DateTime.MinValue, DateTime.MaxValue)
            {
                Conditions = new[]
                {
                    new Condition<ConditionTypeClass>
                    {
                        Type = null,
                        Value = 1
                    }
                }
            };

            SearchArgsValidator<ContentType, ConditionTypeClass> validator = new SearchArgsValidator<ContentType, ConditionTypeClass>();

            // Act
            ValidationResult validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().HaveCount(1);
            validationResult.Errors.Should().Match(c => c.Any(vf => vf.PropertyName == $"{nameof(searchArgs.Conditions)}[0].Type"));
        }

        [Fact]
        public void Validate_GivenConditionWithTypeAsEnumTypeAndDefinedValue_ReturnsSuccessValidation()
        {
            // Arrange
            SearchArgs<ContentType, ConditionType> searchArgs = new SearchArgs<ContentType, ConditionType>(ContentType.Type1, DateTime.MinValue, DateTime.MaxValue)
            {
                Conditions = new[]
                {
                    new Condition<ConditionType>
                    {
                        Type = ConditionType.IsoCountryCode,
                        Value = "PT"
                    }
                }
            };

            SearchArgsValidator<ContentType, ConditionType> validator = new SearchArgsValidator<ContentType, ConditionType>();

            // Act
            ValidationResult validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeTrue();
            validationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_GivenContentTypeAsClassTypeAndNotNullValue_ReturnsSuccessValidation()
        {
            // Arrange
            ContentTypeClass contentType = new ContentTypeClass
            {
                Id = 1,
                Name = "Sample"
            };
            SearchArgs<ContentTypeClass, ConditionType> searchArgs = new SearchArgs<ContentTypeClass, ConditionType>(contentType, DateTime.MinValue, DateTime.MaxValue);

            SearchArgsValidator<ContentTypeClass, ConditionType> validator = new SearchArgsValidator<ContentTypeClass, ConditionType>();

            // Act
            ValidationResult validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeTrue();
            validationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_GivenContentTypeAsClassTypeAndNullValue_ReturnsFailedValidation()
        {
            // Arrange
            SearchArgs<ContentTypeClass, ConditionType> searchArgs = new SearchArgs<ContentTypeClass, ConditionType>(null, DateTime.MinValue, DateTime.MaxValue);

            SearchArgsValidator<ContentTypeClass, ConditionType> validator = new SearchArgsValidator<ContentTypeClass, ConditionType>();

            // Act
            ValidationResult validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().HaveCount(1);
            validationResult.Errors.Should().Match(c => c.Any(vf => vf.PropertyName == nameof(searchArgs.ContentType)));
        }

        [Fact]
        public void Validate_GivenContentTypeAsEnumTypeAndDefinedValue_ReturnsSuccessValidation()
        {
            // Arrange
            SearchArgs<ContentType, ConditionType> searchArgs = new SearchArgs<ContentType, ConditionType>(ContentType.Type1, DateTime.MinValue, DateTime.MaxValue);

            SearchArgsValidator<ContentType, ConditionType> validator = new SearchArgsValidator<ContentType, ConditionType>();

            // Act
            ValidationResult validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeTrue();
            validationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_GivenContentTypeAsEnumTypeAndUndefinedValue_ReturnsFailedValidation()
        {
            // Arrange
            SearchArgs<ContentType, ConditionType> searchArgs = new SearchArgs<ContentType, ConditionType>(0, DateTime.MinValue, DateTime.MaxValue);

            SearchArgsValidator<ContentType, ConditionType> validator = new SearchArgsValidator<ContentType, ConditionType>();

            // Act
            ValidationResult validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().HaveCount(1);
            validationResult.Errors.Should().Match(c => c.Any(vf => vf.PropertyName == nameof(searchArgs.ContentType)));
        }

        [Fact]
        public void Validate_GivenDateEndLesserThanDateEnd_ReturnsFailedValidation()
        {
            // Arrange
            SearchArgs<ContentType, ConditionType> searchArgs = new SearchArgs<ContentType, ConditionType>(ContentType.Type1, DateTime.Parse("2021-03-01Z"), DateTime.Parse("2021-02-01Z"));

            SearchArgsValidator<ContentType, ConditionType> validator = new SearchArgsValidator<ContentType, ConditionType>();

            // Act
            ValidationResult validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().HaveCount(1);
            validationResult.Errors.Should().Match(c => c.Any(vf => vf.PropertyName == nameof(searchArgs.DateEnd)));
        }
    }
}