namespace Rules.Framework.Tests.Evaluation.Compiled
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Evaluation.Compiled;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class ConditionsValueLookupExtensionTests
    {
        [Fact]
        public void GetValueOrDefault_GivenConditionsDictionaryAndConditionType_ReturnsNull()
        {
            // Arrange
            const string expected = "EUR";
            var conditions = new Dictionary<string, object>
            {
                { ConditionNames.IsoCurrency.ToString(), expected }
            };
            var conditionType = ConditionNames.IsoCurrency.ToString();

            // Act
            var result = ConditionsValueLookupExtension.GetValueOrDefault(conditions, conditionType);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void GetValueOrDefault_GivenEmptyConditionsDictionaryAndConditionType_ReturnsNull()
        {
            // Arrange
            var conditions = new Dictionary<string, object>();
            var conditionType = ConditionNames.IsoCurrency.ToString();

            // Act
            var result = ConditionsValueLookupExtension.GetValueOrDefault(conditions, conditionType);

            // Assert
            result.Should().BeNull();
        }
    }
}