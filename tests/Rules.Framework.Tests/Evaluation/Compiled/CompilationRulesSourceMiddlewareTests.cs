namespace Rules.Framework.Tests.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Builder;
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

            var addRuleArgs = new AddRuleArgs<ContentType, ConditionType>
            {
                Rule = expectedRule,
            };

            bool nextDelegateWasInvoked = false;
            var nextDelegate = new AddRuleDelegate<ContentType, ConditionType>((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.CompletedTask;
            });

            Expression<Func<EvaluationContext<ConditionType>, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder<ConditionType>>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware<ContentType, ConditionType>(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            await compilationRulesSourceMiddleware.HandleAddRuleAsync(addRuleArgs, nextDelegate).ConfigureAwait(false);

            // Assert
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()), Times.Never());
        }

        [Fact]
        public async Task HandleAddRuleAsync_GivenRuleWithoutConditionsAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: false);
            var expectedRule = ruleResult.Rule;

            var addRuleArgs = new AddRuleArgs<ContentType, ConditionType>
            {
                Rule = expectedRule,
            };

            bool nextDelegateWasInvoked = false;
            var nextDelegate = new AddRuleDelegate<ContentType, ConditionType>((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.CompletedTask;
            });

            Expression<Func<EvaluationContext<ConditionType>, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder<ConditionType>>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware<ContentType, ConditionType>(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            await compilationRulesSourceMiddleware.HandleAddRuleAsync(addRuleArgs, nextDelegate).ConfigureAwait(false);

            // Assert
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()), Times.Never());
        }

        [Fact]
        public async Task HandleAddRuleAsync_GivenRuleWithUncompiledConditionAndNextDelegate_CompilesAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;

            var addRuleArgs = new AddRuleArgs<ContentType, ConditionType>
            {
                Rule = expectedRule,
            };

            bool nextDelegateWasInvoked = false;
            var nextDelegate = new AddRuleDelegate<ContentType, ConditionType>((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.CompletedTask;
            });

            Expression<Func<EvaluationContext<ConditionType>, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder<ConditionType>>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware<ContentType, ConditionType>(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            await compilationRulesSourceMiddleware.HandleAddRuleAsync(addRuleArgs, nextDelegate).ConfigureAwait(false);

            // Assert
            expectedRule.RootCondition.Properties.Should().HaveCount(2);
            expectedRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.IsCompiledKey)
                .WhoseValue.Should().Be(true);
            expectedRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.CompiledDelegateKey)
                .WhoseValue.Should().BeOfType<Func<EvaluationContext<ConditionType>, bool>>();
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()), Times.Once());
        }

        [Fact]
        public async Task HandleGetRulesAsync_GivenArgsFilteringToRulesWithCompiledConditionsAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;

            // Simulate compiled rule.
            expectedRule.RootCondition.Properties[ConditionNodeProperties.CompilationProperties.IsCompiledKey] = true;
            var expectedRules = new[] { expectedRule };

            var getRulesArgs = new GetRulesArgs<ContentType>
            {
                ContentType = ContentType.Type1,
                DateBegin = DateTime.UtcNow.AddDays(-1),
                DateEnd = DateTime.UtcNow.AddDays(1),
            };

            bool nextDelegateWasInvoked = false;
            var nextDelegate = new GetRulesDelegate<ContentType, ConditionType>((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.FromResult<IEnumerable<Rule<ContentType, ConditionType>>>(expectedRules);
            });

            Expression<Func<EvaluationContext<ConditionType>, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder<ConditionType>>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.IsAny<Rule<ContentType, ConditionType>>()));

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware<ContentType, ConditionType>(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            var actualRules = await compilationRulesSourceMiddleware.HandleGetRulesAsync(getRulesArgs, nextDelegate).ConfigureAwait(false);

            // Assert
            actualRules.Should().BeEquivalentTo(expectedRules);
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()), Times.Never());
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.IsAny<Rule<ContentType, ConditionType>>()), Times.Never());
        }

        [Fact]
        public async Task HandleGetRulesAsync_GivenArgsFilteringToRulesWithoutConditionsAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: false);
            var expectedRule = ruleResult.Rule;
            var expectedRules = new[] { expectedRule };

            var getRulesArgs = new GetRulesArgs<ContentType>
            {
                ContentType = ContentType.Type1,
                DateBegin = DateTime.UtcNow.AddDays(-1),
                DateEnd = DateTime.UtcNow.AddDays(1),
            };

            bool nextDelegateWasInvoked = false;
            var nextDelegate = new GetRulesDelegate<ContentType, ConditionType>((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.FromResult<IEnumerable<Rule<ContentType, ConditionType>>>(expectedRules);
            });

            Expression<Func<EvaluationContext<ConditionType>, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder<ConditionType>>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.IsAny<Rule<ContentType, ConditionType>>()));

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware<ContentType, ConditionType>(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            var actualRules = await compilationRulesSourceMiddleware.HandleGetRulesAsync(getRulesArgs, nextDelegate).ConfigureAwait(false);

            // Assert
            actualRules.Should().BeEquivalentTo(expectedRules);
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()), Times.Never());
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.IsAny<Rule<ContentType, ConditionType>>()), Times.Never());
        }

        [Fact]
        public async Task HandleGetRulesAsync_GivenArgsFilteringToRulesWithUncompiledConditionsAndNextDelegate_CompilesUpdatesAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;
            var expectedRules = new[] { expectedRule };

            var getRulesArgs = new GetRulesArgs<ContentType>
            {
                ContentType = ContentType.Type1,
                DateBegin = DateTime.UtcNow.AddDays(-1),
                DateEnd = DateTime.UtcNow.AddDays(1),
            };

            bool nextDelegateWasInvoked = false;
            var nextDelegate = new GetRulesDelegate<ContentType, ConditionType>((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.FromResult<IEnumerable<Rule<ContentType, ConditionType>>>(expectedRules);
            });

            Expression<Func<EvaluationContext<ConditionType>, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder<ConditionType>>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.IsAny<Rule<ContentType, ConditionType>>()));

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware<ContentType, ConditionType>(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            var actualRules = await compilationRulesSourceMiddleware.HandleGetRulesAsync(getRulesArgs, nextDelegate).ConfigureAwait(false);

            // Assert
            actualRules.Should().HaveCount(1);
            var actualRule = actualRules.First();
            actualRule.RootCondition.Properties.Should().HaveCount(2);
            actualRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.IsCompiledKey)
                .WhoseValue.Should().Be(true);
            actualRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.CompiledDelegateKey)
                .WhoseValue.Should().BeOfType<Func<EvaluationContext<ConditionType>, bool>>();
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()), Times.Once());
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.IsAny<Rule<ContentType, ConditionType>>()), Times.Once());
        }

        [Fact]
        public async Task HandleGetRulesFilteredAsync_GivenArgsFilteringToRulesWithCompiledConditionsAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;

            // Simulate compiled rule.
            expectedRule.RootCondition.Properties[ConditionNodeProperties.CompilationProperties.IsCompiledKey] = true;
            var expectedRules = new[] { expectedRule };

            var getRulesFilteredArgs = new GetRulesFilteredArgs<ContentType>
            {
                ContentType = ContentType.Type1,
            };

            bool nextDelegateWasInvoked = false;
            var nextDelegate = new GetRulesFilteredDelegate<ContentType, ConditionType>((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.FromResult<IEnumerable<Rule<ContentType, ConditionType>>>(expectedRules);
            });

            Expression<Func<EvaluationContext<ConditionType>, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder<ConditionType>>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.IsAny<Rule<ContentType, ConditionType>>()));

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware<ContentType, ConditionType>(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            var actualRules = await compilationRulesSourceMiddleware.HandleGetRulesFilteredAsync(getRulesFilteredArgs, nextDelegate).ConfigureAwait(false);

            // Assert
            actualRules.Should().BeEquivalentTo(expectedRules);
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()), Times.Never());
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.IsAny<Rule<ContentType, ConditionType>>()), Times.Never());
        }

        [Fact]
        public async Task HandleGetRulesFilteredAsync_GivenArgsFilteringToRulesWithoutConditionsAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: false);
            var expectedRule = ruleResult.Rule;
            var expectedRules = new[] { expectedRule };

            var getRulesFilteredArgs = new GetRulesFilteredArgs<ContentType>
            {
                ContentType = ContentType.Type1,
            };

            bool nextDelegateWasInvoked = false;
            var nextDelegate = new GetRulesFilteredDelegate<ContentType, ConditionType>((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.FromResult<IEnumerable<Rule<ContentType, ConditionType>>>(expectedRules);
            });

            Expression<Func<EvaluationContext<ConditionType>, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder<ConditionType>>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.IsAny<Rule<ContentType, ConditionType>>()));

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware<ContentType, ConditionType>(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            var actualRules = await compilationRulesSourceMiddleware.HandleGetRulesFilteredAsync(getRulesFilteredArgs, nextDelegate).ConfigureAwait(false);

            // Assert
            actualRules.Should().BeEquivalentTo(expectedRules);
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()), Times.Never());
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.IsAny<Rule<ContentType, ConditionType>>()), Times.Never());
        }

        [Fact]
        public async Task HandleGetRulesFilteredAsync_GivenArgsFilteringToRulesWithUncompiledConditionsAndNextDelegate_CompilesUpdatesAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;
            var expectedRules = new[] { expectedRule };

            var getRulesFilteredArgs = new GetRulesFilteredArgs<ContentType>
            {
                ContentType = ContentType.Type1,
            };

            bool nextDelegateWasInvoked = false;
            var nextDelegate = new GetRulesFilteredDelegate<ContentType, ConditionType>((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.FromResult<IEnumerable<Rule<ContentType, ConditionType>>>(expectedRules);
            });

            Expression<Func<EvaluationContext<ConditionType>, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder<ConditionType>>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            Mock.Get(rulesDataSource)
                .Setup(x => x.UpdateRuleAsync(It.IsAny<Rule<ContentType, ConditionType>>()));

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware<ContentType, ConditionType>(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            var actualRules = await compilationRulesSourceMiddleware.HandleGetRulesFilteredAsync(getRulesFilteredArgs, nextDelegate).ConfigureAwait(false);

            // Assert
            actualRules.Should().HaveCount(1);
            var actualRule = actualRules.First();
            actualRule.RootCondition.Properties.Should().HaveCount(2);
            actualRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.IsCompiledKey)
                .WhoseValue.Should().Be(true);
            actualRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.CompiledDelegateKey)
                .WhoseValue.Should().BeOfType<Func<EvaluationContext<ConditionType>, bool>>();
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()), Times.Once());
            Mock.Get(rulesDataSource)
                .Verify(x => x.UpdateRuleAsync(It.IsAny<Rule<ContentType, ConditionType>>()), Times.Once());
        }

        [Fact]
        public async Task HandleUpdateRuleAsync_GivenRuleWithCompiledConditionAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;

            // Simulate compiled rule.
            expectedRule.RootCondition.Properties[ConditionNodeProperties.CompilationProperties.IsCompiledKey] = true;

            var updateRuleArgs = new UpdateRuleArgs<ContentType, ConditionType>
            {
                Rule = expectedRule,
            };

            bool nextDelegateWasInvoked = false;
            var nextDelegate = new UpdateRuleDelegate<ContentType, ConditionType>((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.CompletedTask;
            });

            Expression<Func<EvaluationContext<ConditionType>, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder<ConditionType>>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware<ContentType, ConditionType>(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            await compilationRulesSourceMiddleware.HandleUpdateRuleAsync(updateRuleArgs, nextDelegate).ConfigureAwait(false);

            // Assert
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()), Times.Never());
        }

        [Fact]
        public async Task HandleUpdateRuleAsync_GivenRuleWithoutConditionsAndNextDelegate_IgnoresAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: false);
            var expectedRule = ruleResult.Rule;

            var updateRuleArgs = new UpdateRuleArgs<ContentType, ConditionType>
            {
                Rule = expectedRule,
            };

            bool nextDelegateWasInvoked = false;
            var nextDelegate = new UpdateRuleDelegate<ContentType, ConditionType>((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.CompletedTask;
            });

            Expression<Func<EvaluationContext<ConditionType>, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder<ConditionType>>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware<ContentType, ConditionType>(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            await compilationRulesSourceMiddleware.HandleUpdateRuleAsync(updateRuleArgs, nextDelegate).ConfigureAwait(false);

            // Assert
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()), Times.Never());
        }

        [Fact]
        public async Task HandleUpdateRuleAsync_GivenRuleWithUncompiledConditionAndNextDelegate_CompilesAndProceedsToNextDelegate()
        {
            // Arrange
            var ruleResult = CreateTestRule(withCondition: true);
            var expectedRule = ruleResult.Rule;

            var updateRuleArgs = new UpdateRuleArgs<ContentType, ConditionType>
            {
                Rule = expectedRule,
            };

            bool nextDelegateWasInvoked = false;
            var nextDelegate = new UpdateRuleDelegate<ContentType, ConditionType>((_) =>
            {
                nextDelegateWasInvoked = true;
                return Task.CompletedTask;
            });

            Expression<Func<EvaluationContext<ConditionType>, bool>> expectedExpression = (_) => true;

            var ruleConditionsExpressionBuilder = Mock.Of<IRuleConditionsExpressionBuilder<ConditionType>>();
            Mock.Get(ruleConditionsExpressionBuilder)
                .Setup(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()))
                .Returns(expectedExpression);

            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();

            var compilationRulesSourceMiddleware = new CompilationRulesSourceMiddleware<ContentType, ConditionType>(
                ruleConditionsExpressionBuilder,
                rulesDataSource);

            // Act
            await compilationRulesSourceMiddleware.HandleUpdateRuleAsync(updateRuleArgs, nextDelegate).ConfigureAwait(false);

            // Assert
            expectedRule.RootCondition.Properties.Should().HaveCount(2);
            expectedRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.IsCompiledKey)
                .WhoseValue.Should().Be(true);
            expectedRule.RootCondition.Properties.Should().ContainKey(ConditionNodeProperties.CompilationProperties.CompiledDelegateKey)
                .WhoseValue.Should().BeOfType<Func<EvaluationContext<ConditionType>, bool>>();
            nextDelegateWasInvoked.Should().BeTrue();

            Mock.Get(ruleConditionsExpressionBuilder)
                .Verify(x => x.BuildExpression(It.IsAny<IConditionNode<ConditionType>>()), Times.Once());
        }

        private static RuleBuilderResult<ContentType, ConditionType> CreateTestRule(bool withCondition)
        {
            var ruleBuilder = RuleBuilder.NewRule<ContentType, ConditionType>()
                        .WithName("Test rule")
                        .WithDateBegin(DateTime.UtcNow)
                        .WithContentContainer(new ContentContainer<ContentType>(ContentType.Type1, t => "Test content"));

            if (withCondition)
            {
                ruleBuilder.WithCondition(x =>
                    x.AsValued(ConditionType.IsoCountryCode)
                        .OfDataType<string>()
                        .WithComparisonOperator(Operators.Equal)
                        .SetOperand("PT")
                        .Build());
            }

            return ruleBuilder.Build();
        }
    }
}