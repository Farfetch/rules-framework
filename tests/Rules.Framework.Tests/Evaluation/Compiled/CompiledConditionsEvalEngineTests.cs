namespace Rules.Framework.Tests.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Rules.Framework;
    using Rules.Framework.Builder.Generic;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class CompiledConditionsEvalEngineTests
    {
        [Fact]
        public void Eval_GivenCompiledConditionNodeConditionsAndExcludeRulesWithoutSearchConditions_DoesNotHaveAllConditionsAndReturnsFalse()
        {
            // Arrange
            var ruleResult = CreateTestRule();
            var expectedRule = (Rule)ruleResult.Rule;
            Func<EvaluationContext, bool> expectedExpression = (_) => true;
            expectedRule.RootCondition.Properties[ConditionNodeProperties.CompilationProperties.CompiledDelegateKey] = expectedExpression;
            var conditions = new Dictionary<string, object>();
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = true,
                MatchMode = MatchModes.Exact,
            };

            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer>();
            Mock.Get(conditionsTreeAnalyzer)
                .Setup(x => x.AreAllSearchConditionsPresent(It.IsAny<IConditionNode>(), It.IsAny<IDictionary<string, object>>()))
                .Returns(false);

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var compiledConditionsEvalEngine = new CompiledConditionsEvalEngine(conditionsTreeAnalyzer, rulesEngineOptions);

            // Act
            var result = compiledConditionsEvalEngine.Eval(expectedRule.RootCondition, conditions, evaluationOptions);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Eval_GivenCompiledConditionNodeConditionsAndExcludeRulesWithoutSearchConditions_ExecutesEvaluationAndReturnsResult()
        {
            // Arrange
            var ruleResult = CreateTestRule();
            var expectedRule = (Rule)ruleResult.Rule;
            Func<EvaluationContext, bool> expectedExpression = (_) => true;
            expectedRule.RootCondition.Properties[ConditionNodeProperties.CompilationProperties.CompiledDelegateKey] = expectedExpression;
            var conditions = new Dictionary<string, object>();
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = true,
                MatchMode = MatchModes.Exact,
            };

            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer>();
            Mock.Get(conditionsTreeAnalyzer)
                .Setup(x => x.AreAllSearchConditionsPresent(It.IsAny<IConditionNode>(), It.IsAny<IDictionary<string, object>>()))
                .Returns(true);

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var compiledConditionsEvalEngine = new CompiledConditionsEvalEngine(conditionsTreeAnalyzer, rulesEngineOptions);

            // Act
            var result = compiledConditionsEvalEngine.Eval(expectedRule.RootCondition, conditions, evaluationOptions);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Eval_GivenCompiledConditionNodeConditionsAndIncludeRulesWithoutSearchConditions_ExecutesEvaluationAndReturnsResult()
        {
            // Arrange
            var ruleResult = CreateTestRule();
            var expectedRule = (Rule)ruleResult.Rule;
            Func<EvaluationContext, bool> expectedExpression = (_) => true;
            expectedRule.RootCondition.Properties[ConditionNodeProperties.CompilationProperties.CompiledDelegateKey] = expectedExpression;
            var conditions = new Dictionary<string, object>();
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact,
            };

            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer>();

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var compiledConditionsEvalEngine = new CompiledConditionsEvalEngine(conditionsTreeAnalyzer, rulesEngineOptions);

            // Act
            var result = compiledConditionsEvalEngine.Eval(expectedRule.RootCondition, conditions, evaluationOptions);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Eval_GivenUncompiledConditionNodeConditionsAndIncludeRulesWithoutSearchConditions_ThrowsArgumentException()
        {
            // Arrange
            var ruleResult = CreateTestRule();
            var expectedRule = (Rule)ruleResult.Rule;
            var conditions = new Dictionary<string, object>();
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact,
            };

            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer>();

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var compiledConditionsEvalEngine = new CompiledConditionsEvalEngine(conditionsTreeAnalyzer, rulesEngineOptions);

            // Act
            var action = FluentActions.Invoking(() => compiledConditionsEvalEngine.Eval(expectedRule.RootCondition, conditions, evaluationOptions));

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .Which.ParamName.Should().Be("conditionNode");
        }

        private static RuleBuilderResult<ContentType, ConditionType> CreateTestRule() => Rule.New<ContentType, ConditionType>()
            .WithName("Test rule")
            .WithDateBegin(DateTime.UtcNow)
            .WithContent(ContentType.Type1, "Test content")
            .WithCondition(ConditionType.IsoCurrency, Operators.Equal, "EUR")
            .Build();
    }
}