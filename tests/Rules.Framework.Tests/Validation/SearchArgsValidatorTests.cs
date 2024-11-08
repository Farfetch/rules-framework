namespace Rules.Framework.Tests.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Rules.Framework.Tests.Stubs;
    using Rules.Framework.Validation;
    using Xunit;

    public class SearchArgsValidatorTests
    {
        [Fact]
        public void Validate_GivenConditionWithTypeAsAsEnumTypeAndUndefinedValue_ReturnsFailedValidation()
        {
            // Arrange
            var searchArgs = new SearchArgs<RulesetNames, ConditionNames>(RulesetNames.Type1, DateTime.MinValue, DateTime.MaxValue)
            {
                Conditions = new Dictionary<ConditionNames, object>
                {
                    { 0, 1 },
                },
            };

            var validator = new SearchArgsValidator<RulesetNames, ConditionNames>();

            // Act
            var validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().HaveCount(1);
            validationResult.Errors.Should().Match(c => c.Any(vf => vf.PropertyName == $"{nameof(searchArgs.Conditions)}[0].Key"));
        }

        [Fact]
        public void Validate_GivenConditionWithTypeAsClassTypeAndNotNullValue_ReturnsSuccessValidation()
        {
            // Arrange
            var searchArgs = new SearchArgs<RulesetNames, ConditionClass>(RulesetNames.Type1, DateTime.MinValue, DateTime.MaxValue)
            {
                Conditions = new Dictionary<ConditionClass, object>
                {
                    { new ConditionClass { Id = 1, Name = "Sample Condition Type" }, 1 },
                },
            };

            var validator = new SearchArgsValidator<RulesetNames, ConditionClass>();

            // Act
            var validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeTrue();
            validationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_GivenConditionWithTypeAsEnumTypeAndDefinedValue_ReturnsSuccessValidation()
        {
            // Arrange
            var searchArgs = new SearchArgs<RulesetNames, ConditionNames>(RulesetNames.Type1, DateTime.MinValue, DateTime.MaxValue)
            {
                Conditions = new Dictionary<ConditionNames, object>
                {
                    { ConditionNames.IsoCountryCode, "PT" },
                },
            };

            var validator = new SearchArgsValidator<RulesetNames, ConditionNames>();

            // Act
            var validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeTrue();
            validationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_GivenContentTypeAsClassTypeAndNotNullValue_ReturnsSuccessValidation()
        {
            // Arrange
            var contentType = new RulesetClass
            {
                Id = 1,
                Name = "Sample"
            };
            var searchArgs = new SearchArgs<RulesetClass, ConditionNames>(contentType, DateTime.MinValue, DateTime.MaxValue);

            var validator = new SearchArgsValidator<RulesetClass, ConditionNames>();

            // Act
            var validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeTrue();
            validationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_GivenContentTypeAsClassTypeAndNullValue_ReturnsFailedValidation()
        {
            // Arrange
            var searchArgs = new SearchArgs<RulesetClass, ConditionNames>(null, DateTime.MinValue, DateTime.MaxValue);

            var validator = new SearchArgsValidator<RulesetClass, ConditionNames>();

            // Act
            var validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().HaveCount(1);
            validationResult.Errors.Should().Match(c => c.Any(vf => vf.PropertyName == nameof(searchArgs.Ruleset)));
        }

        [Fact]
        public void Validate_GivenContentTypeAsEnumTypeAndDefinedValue_ReturnsSuccessValidation()
        {
            // Arrange
            var searchArgs = new SearchArgs<RulesetNames, ConditionNames>(RulesetNames.Type1, DateTime.MinValue, DateTime.MaxValue);

            var validator = new SearchArgsValidator<RulesetNames, ConditionNames>();

            // Act
            var validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeTrue();
            validationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_GivenContentTypeAsEnumTypeAndUndefinedValue_ReturnsFailedValidation()
        {
            // Arrange
            var searchArgs = new SearchArgs<RulesetNames, ConditionNames>(0, DateTime.MinValue, DateTime.MaxValue);

            var validator = new SearchArgsValidator<RulesetNames, ConditionNames>();

            // Act
            var validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().HaveCount(1);
            validationResult.Errors.Should().Match(c => c.Any(vf => vf.PropertyName == nameof(searchArgs.Ruleset)));
        }

        [Fact]
        public void Validate_GivenDateEndLesserThanDateEnd_ReturnsFailedValidation()
        {
            // Arrange
            var searchArgs = new SearchArgs<RulesetNames, ConditionNames>(RulesetNames.Type1, DateTime.Parse("2021-03-01Z"), DateTime.Parse("2021-02-01Z"));

            var validator = new SearchArgsValidator<RulesetNames, ConditionNames>();

            // Act
            var validationResult = validator.Validate(searchArgs);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().HaveCount(1);
            validationResult.Errors.Should().Match(c => c.Any(vf => vf.PropertyName == nameof(searchArgs.DateEnd)));
        }
    }
}