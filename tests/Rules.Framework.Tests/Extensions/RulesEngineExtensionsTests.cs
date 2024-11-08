namespace Rules.Framework.Tests.Extensions
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Generic;
    using Rules.Framework.Source;
    using Rules.Framework.Tests.Stubs;
    using Rules.Framework.Validation;
    using Xunit;

    public class RulesEngineExtensionsTests
    {
        [Fact]
        public void RulesEngineExtensions_MakeGeneric_ReturnsGenericRulesEngine()
        {
            // Arrange
            var rulesEngine = new RulesEngine(
                Mock.Of<IConditionsEvalEngine>(),
                Mock.Of<IRulesSource>(),
                Mock.Of<IValidatorProvider>(),
                RulesEngineOptions.NewWithDefaults(),
                Mock.Of<IRuleConditionsExtractor>());

            // Act
            var genericEngine = rulesEngine.MakeGeneric<RulesetNames, ConditionNames>();

            // Assert
            genericEngine.Should().NotBeNull();
            genericEngine.GetType().Should().Be(typeof(RulesEngine<RulesetNames, ConditionNames>));
        }
    }
}