namespace Rules.Framework.Tests.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class CompiledConditionsEvalEngineTests
    {
        [Fact]
        public void Eval_GivenCompiledConditionNodeConditionsAndExcludeRulesWithoutSearchConditions_DoesNotHaveAllConditionsAndReturnsFalse()
        {
            // Arrange
            var ruleResult = CreateTestRule();
            var expectedRule = ruleResult.Rule;
            Func<EvaluationContext<ConditionType>, bool> expectedExpression = (evaluationContext) => true;
            expectedRule.RootCondition.Properties[ConditionNodeProperties.CompilationProperties.CompiledDelegateKey] = expectedExpression;
            var conditions = new Dictionary<ConditionType, object>();
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = true,
                MatchMode = MatchModes.Exact,
            };

            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer<ConditionType>>();
            Mock.Get(conditionsTreeAnalyzer)
                .Setup(x => x.AreAllSearchConditionsPresent(It.IsAny<IConditionNode<ConditionType>>(), It.IsAny<IDictionary<ConditionType, object>>()))
                .Returns(false);

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var compiledConditionsEvalEngine = new CompiledConditionsEvalEngine<ConditionType>(conditionsTreeAnalyzer, rulesEngineOptions);

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
            var expectedRule = ruleResult.Rule;
            Func<EvaluationContext<ConditionType>, bool> expectedExpression = (evaluationContext) => true;
            expectedRule.RootCondition.Properties[ConditionNodeProperties.CompilationProperties.CompiledDelegateKey] = expectedExpression;
            var conditions = new Dictionary<ConditionType, object>();
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = true,
                MatchMode = MatchModes.Exact,
            };

            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer<ConditionType>>();
            Mock.Get(conditionsTreeAnalyzer)
                .Setup(x => x.AreAllSearchConditionsPresent(It.IsAny<IConditionNode<ConditionType>>(), It.IsAny<IDictionary<ConditionType, object>>()))
                .Returns(true);

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var compiledConditionsEvalEngine = new CompiledConditionsEvalEngine<ConditionType>(conditionsTreeAnalyzer, rulesEngineOptions);

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
            var expectedRule = ruleResult.Rule;
            Func<EvaluationContext<ConditionType>, bool> expectedExpression = (evaluationContext) => true;
            expectedRule.RootCondition.Properties[ConditionNodeProperties.CompilationProperties.CompiledDelegateKey] = expectedExpression;
            var conditions = new Dictionary<ConditionType, object>();
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact,
            };

            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer<ConditionType>>();

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var compiledConditionsEvalEngine = new CompiledConditionsEvalEngine<ConditionType>(conditionsTreeAnalyzer, rulesEngineOptions);

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
            var expectedRule = ruleResult.Rule;
            var conditions = new Dictionary<ConditionType, object>();
            var evaluationOptions = new EvaluationOptions
            {
                ExcludeRulesWithoutSearchConditions = false,
                MatchMode = MatchModes.Exact,
            };

            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer<ConditionType>>();

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var compiledConditionsEvalEngine = new CompiledConditionsEvalEngine<ConditionType>(conditionsTreeAnalyzer, rulesEngineOptions);

            // Act
            var action = FluentActions.Invoking(() => compiledConditionsEvalEngine.Eval(expectedRule.RootCondition, conditions, evaluationOptions));

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .Which.ParamName.Should().Be("conditionNode");
        }

        private static RuleBuilderResult<ContentType, ConditionType> CreateTestRule() => RuleBuilder.NewRule<ContentType, ConditionType>()
            .WithName("Test rule")
            .WithDateBegin(DateTime.UtcNow)
            .WithContentContainer(new ContentContainer<ContentType>(ContentType.Type1, t => "Test content"))
            .WithCondition(x =>
                x.AsValued(ConditionType.IsoCurrency)
                    .OfDataType<string>()
                    .WithComparisonOperator(Operators.Equal)
                    .SetOperand("EUR")
                    .Build())
            .Build();
    }
}