namespace Rules.Framework.Tests.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework;
    using Rules.Framework.Builder.Generic;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation.Compiled;
    using Rules.Framework.Source;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class CompilationRulesSourceMiddlewareTests
    {
        [Fact]
        public async Task HandleAddRuleAsync_GivenRuleWithCompiledConditionAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;

            // Simulate compiled rule.
            expectedRule.RootCondition.Properties[ConditionNodeProperties.CompilationProperties.IsCompiledKey] = true;

            var addRuleArgs = new AddRuleArgs
            {
                Rule = expectedRule,
            };

            var nextDelegateWasInvoked = false;
            var nextDelegate = new AddRuleDelegate((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.CompletedTask;
            });

            Expression<Func<EvaluationContext, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource>();

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            await compilationRulesSourceMiddleware.HandleAddRuleAsync(addRuleArgs, nextDelegate);

            // Assert
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode>()), Times.Never());
        }

        [Fact]
        public async Task HandleAddRuleAsync_GivenRuleWithoutConditionsAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: false);
            var expectedRule = ruleResult.Rule;

            var addRuleArgs = new AddRuleArgs
            {
                Rule = expectedRule,
            };

            var nextDelegateWasInvoked = false;
            var nextDelegate = new AddRuleDelegate((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.CompletedTask;
            });

            Expression<Func<EvaluationContext, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource>();

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            await compilationRulesSourceMiddleware.HandleAddRuleAsync(addRuleArgs, nextDelegate);

            // Assert
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode>()), Times.Never());
        }

        [Fact]
        public async Task HandleAddRuleAsync_GivenRuleWithUncompiledConditionAndNextDelegate_CompilesAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;

            var addRuleArgs = new AddRuleArgs
            {
                Rule = expectedRule,
            };

            var nextDelegateWasInvoked = false;
            var nextDelegate = new AddRuleDelegate((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.CompletedTask;
            });

            Expression<Func<EvaluationContext, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource>();

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            await compilationRulesSourceMiddleware.HandleAddRuleAsync(addRuleArgs, nextDelegate);

            // Assert
            expectedRule.RootCondition.Properties.Should().HaveCount(2);
            expectedRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.IsCompiledKey)
                .WhoseValue.Should().Be(true);
            expectedRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.CompiledDelegateKey)
                .WhoseValue.Should().BeOfType<Func<EvaluationContext, bool>>();
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.VerifyAll(
                Mock.Get(ruleConditionsExpressionBuilder));
        }

        [Fact]
        public async Task HandleGetRulesAsync_GivenArgsFilteringToRulesWithCompiledConditionsAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;

            // Simulate compiled rule.
            expectedRule.RootCondition.Properties[ConditionNodeProperties.CompilationProperties.IsCompiledKey] = true;
            var expectedRules = new Rule[] { expectedRule };

            var getRulesArgs = new GetRulesArgs
            {
                ContentType = RulesetNames.Type1.ToString(),
                DateBegin = DateTime.UtcNow.AddDays(-1),
                DateEnd = DateTime.UtcNow.AddDays(1),
            };

            var nextDelegateWasInvoked = false;
            var nextDelegate = new GetRulesDelegate((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.FromResult<IEnumerable<Rule>>(expectedRules);
            });

            Expression<Func<EvaluationContext, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.IsAny<Rule>()));

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            var actualRules = await compilationRulesSourceMiddleware.HandleGetRulesAsync(getRulesArgs, nextDelegate);

            // Assert
            actualRules.Should().BeEquivalentTo(expectedRules);
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode>()), Times.Never());
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.IsAny<Rule>()), Times.Never());
        }

        [Fact]
        public async Task HandleGetRulesAsync_GivenArgsFilteringToRulesWithoutConditionsAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: false);
            var expectedRule = ruleResult.Rule;
            var expectedRules = new Rule[] { expectedRule };

            var getRulesArgs = new GetRulesArgs
            {
                ContentType = RulesetNames.Type1.ToString(),
                DateBegin = DateTime.UtcNow.AddDays(-1),
                DateEnd = DateTime.UtcNow.AddDays(1),
            };

            var nextDelegateWasInvoked = false;
            var nextDelegate = new GetRulesDelegate((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.FromResult<IEnumerable<Rule>>(expectedRules);
            });

            Expression<Func<EvaluationContext, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.IsAny<Rule>()));

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            var actualRules = await compilationRulesSourceMiddleware.HandleGetRulesAsync(getRulesArgs, nextDelegate);

            // Assert
            actualRules.Should().BeEquivalentTo(expectedRules);
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode>()), Times.Never());
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.IsAny<Rule>()), Times.Never());
        }

        [Fact]
        public async Task HandleGetRulesAsync_GivenArgsFilteringToRulesWithUncompiledConditionsAndNextDelegate_CompilesUpdatesAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;
            var expectedRules = new Rule[] { expectedRule };

            var getRulesArgs = new GetRulesArgs
            {
                ContentType = RulesetNames.Type1.ToString(),
                DateBegin = DateTime.UtcNow.AddDays(-1),
                DateEnd = DateTime.UtcNow.AddDays(1),
            };

            var nextDelegateWasInvoked = false;
            var nextDelegate = new GetRulesDelegate((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.FromResult<IEnumerable<Rule>>(expectedRules);
            });

            Expression<Func<EvaluationContext, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.IsAny<Rule>()));

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            var actualRules = await compilationRulesSourceMiddleware.HandleGetRulesAsync(getRulesArgs, nextDelegate);

            // Assert
            actualRules.Should().HaveCount(1);
            var actualRule = actualRules.First();
            actualRule.RootCondition.Properties.Should().HaveCount(2);
            actualRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.IsCompiledKey)
                .WhoseValue.Should().Be(true);
            actualRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.CompiledDelegateKey)
                .WhoseValue.Should().BeOfType<Func<EvaluationContext, bool>>();
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.VerifyAll(
                Mock.Get(ruleConditionsExpressionBuilder),
                Mock.Get(rulesDataSource));
        }

        [Fact]
        public async Task HandleGetRulesFilteredAsync_GivenArgsFilteringToRulesWithCompiledConditionsAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;

            // Simulate compiled rule.
            expectedRule.RootCondition.Properties[ConditionNodeProperties.CompilationProperties.IsCompiledKey] = true;
            var expectedRules = new Rule[] { expectedRule };

            var getRulesFilteredArgs = new GetRulesFilteredArgs
            {
                Ruleset = RulesetNames.Type1.ToString(),
            };

            var nextDelegateWasInvoked = false;
            var nextDelegate = new GetRulesFilteredDelegate((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.FromResult<IEnumerable<Rule>>(expectedRules);
            });

            Expression<Func<EvaluationContext, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.IsAny<Rule>()));

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            var actualRules = await compilationRulesSourceMiddleware.HandleGetRulesFilteredAsync(getRulesFilteredArgs, nextDelegate);

            // Assert
            actualRules.Should().BeEquivalentTo(expectedRules);
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode>()), Times.Never());
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.IsAny<Rule>()), Times.Never());
        }

        [Fact]
        public async Task HandleGetRulesFilteredAsync_GivenArgsFilteringToRulesWithoutConditionsAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: false);
            var expectedRule = ruleResult.Rule;
            var expectedRules = new Rule[] { expectedRule };

            var getRulesFilteredArgs = new GetRulesFilteredArgs
            {
                Ruleset = RulesetNames.Type1.ToString(),
            };

            var nextDelegateWasInvoked = false;
            var nextDelegate = new GetRulesFilteredDelegate((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.FromResult<IEnumerable<Rule>>(expectedRules);
            });

            Expression<Func<EvaluationContext, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.IsAny<Rule>()));

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            var actualRules = await compilationRulesSourceMiddleware.HandleGetRulesFilteredAsync(getRulesFilteredArgs, nextDelegate);

            // Assert
            actualRules.Should().BeEquivalentTo(expectedRules);
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode>()), Times.Never());
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.IsAny<Rule>()), Times.Never());
        }

        [Fact]
        public async Task HandleGetRulesFilteredAsync_GivenArgsFilteringToRulesWithUncompiledConditionsAndNextDelegate_CompilesUpdatesAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;
            var expectedRules = new Rule[] { expectedRule };

            var getRulesFilteredArgs = new GetRulesFilteredArgs
            {
                Ruleset = RulesetNames.Type1.ToString(),
            };

            var nextDelegateWasInvoked = false;
            var nextDelegate = new GetRulesFilteredDelegate((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.FromResult<IEnumerable<Rule>>(expectedRules);
            });

            Expression<Func<EvaluationContext, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.IsAny<Rule>()));

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            var actualRules = await compilationRulesSourceMiddleware.HandleGetRulesFilteredAsync(getRulesFilteredArgs, nextDelegate);

            // Assert
            actualRules.Should().HaveCount(1);
            var actualRule = actualRules.First();
            actualRule.RootCondition.Properties.Should().HaveCount(2);
            actualRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.IsCompiledKey)
                .WhoseValue.Should().Be(true);
            actualRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.CompiledDelegateKey)
                .WhoseValue.Should().BeOfType<Func<EvaluationContext, bool>>();
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.VerifyAll(
                Mock.Get(ruleConditionsExpressionBuilder),
                Mock.Get(rulesDataSource));
        }

        [Fact]
        public async Task HandleUpdateRuleAsync_GivenRuleWithCompiledConditionAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;

            // Simulate compiled rule.
            expectedRule.RootCondition.Properties[ConditionNodeProperties.CompilationProperties.IsCompiledKey] = true;

            var updateRuleArgs = new UpdateRuleArgs
            {
                Rule = expectedRule,
            };

            var nextDelegateWasInvoked = false;
            var nextDelegate = new UpdateRuleDelegate((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.CompletedTask;
            });

            Expression<Func<EvaluationContext, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource>();

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            await compilationRulesSourceMiddleware.HandleUpdateRuleAsync(updateRuleArgs, nextDelegate);

            // Assert
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode>()), Times.Never());
        }

        [Fact]
        public async Task HandleUpdateRuleAsync_GivenRuleWithoutConditionsAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: false);
            var expectedRule = ruleResult.Rule;

            var updateRuleArgs = new UpdateRuleArgs
            {
                Rule = expectedRule,
            };

            var nextDelegateWasInvoked = false;
            var nextDelegate = new UpdateRuleDelegate((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.CompletedTask;
            });

            Expression<Func<EvaluationContext, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource>();

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            await compilationRulesSourceMiddleware.HandleUpdateRuleAsync(updateRuleArgs, nextDelegate);

            // Assert
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode>()), Times.Never());
        }

        [Fact]
        public async Task HandleUpdateRuleAsync_GivenRuleWithUncompiledConditionAndNextDelegate_CompilesAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;

            var updateRuleArgs = new UpdateRuleArgs
            {
                Rule = expectedRule,
            };

            var nextDelegateWasInvoked = false;
            var nextDelegate = new UpdateRuleDelegate((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.CompletedTask;
            });

            Expression<Func<EvaluationContext, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource>();

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            await compilationRulesSourceMiddleware.HandleUpdateRuleAsync(updateRuleArgs, nextDelegate);

            // Assert
            expectedRule.RootCondition.Properties.Should().HaveCount(2);
            expectedRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.IsCompiledKey)
                .WhoseValue.Should().Be(true);
            expectedRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.CompiledDelegateKey)
                .WhoseValue.Should().BeOfType<Func<EvaluationContext, bool>>();
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.VerifyAll(
                Mock.Get(ruleConditionsExpressionBuilder));
        }

        private static RuleBuilderResult<RulesetNames, ConditionNames> CreateTestRule(bool withCondition)
        {
            var ruleBuilder = Rule.Create<RulesetNames, ConditionNames>("Test rule")
                .InRuleset(RulesetNames.Type1)
                .SetContent("Test content")
                .Since(DateTime.UtcNow);

            if (withCondition)
            {
                ruleBuilder.ApplyWhen(ConditionNames.IsoCountryCode, Operators.Equal, "PT");
            }

            return ruleBuilder.Build();
        }
    }
}