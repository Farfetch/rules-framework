namespace Rules.Framework.Tests.Extensions
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class TypeExtensionsTests
    {
        public static IEnumerable<object[]> SuccessCases
            => Enum.GetValues<LanguageOperator>().Where(lo => lo != LanguageOperator.None).Select(lo => new object[] { lo });

        [Theory]
        [MemberData(nameof(SuccessCases))]
        public void HasLanguageOperator_GivenMappedLanguageOperator_ReturnsTrue(object langOperator)
        {
            // Arrange
            LanguageOperator languageOperator = (LanguageOperator)langOperator;
            Type type = typeof(int);

            // Act
            bool result = type.HasLanguageOperator(languageOperator);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasLanguageOperator_GivenUnmappedLanguageOperator_ReturnsFalse()
        {
            // Arrange
            LanguageOperator languageOperator = (LanguageOperator)0;
            Type type = typeof(int);

            // Act
            bool result = type.HasLanguageOperator(languageOperator);

            // Assert
            result.Should().BeFalse();
        }
    }
}
