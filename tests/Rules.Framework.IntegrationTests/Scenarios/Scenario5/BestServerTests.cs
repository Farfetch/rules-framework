namespace Rules.Framework.IntegrationTests.Scenarios.Scenario5
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario5;
    using Xunit;

    public class BestServerTests
    {
        [Fact]
        public async Task BestServer_DeactivatingBestServerTop5_ReturnsBestServerDefault()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource<BestServerConfigurations, BestServerConditions>(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                 .WithContentType<BestServerConfigurations>()
                 .WithConditionType<BestServerConditions>()
                 .SetInMemoryDataSource(serviceProvider)
                 .Build();

            // Act 1 - Create rule with "in" operator
            var ruleBuilderResult = RuleBuilder.NewRule<BestServerConfigurations, BestServerConditions>()
                .WithName("Best Server Top5")
                .WithDatesInterval(DateTime.Parse("2021-05-29T11:00:00Z"), DateTime.Parse("2021-05-31Z"))
                .WithContent(BestServerConfigurations.BestServerEvaluation, "Top5")
                .WithCondition(c => c
                    .And(a => a
                        .Value(BestServerConditions.Price, Operators.In, new[] { 100m, 200m, 300m })
                        .Value(BestServerConditions.Memory, Operators.In, new[] { 12, 16, 24, 36 })
                        .Value(BestServerConditions.Memory, Operators.NotIn, new[] { 4, 8 })
                        .Value(BestServerConditions.StoragePartionable, Operators.In, new[] { true })
                        .Value(BestServerConditions.Brand, Operators.In, new[] { "AMD", "Intel", "Cisco" })
                        ))
                .Build();

            // Assert 1
            ruleBuilderResult.Should().NotBeNull();
            var errors = ruleBuilderResult.Errors.Any() ? ruleBuilderResult.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 2 - Add new rule with "in" operator
            var rule = ruleBuilderResult.Rule;

            var addRuleResult = await rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop).ConfigureAwait(false);

            // Assert 2 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 3 - Create rule default
            var ruleBuilderResultDefault = RuleBuilder
                .NewRule<BestServerConfigurations, BestServerConditions>()
                .WithName("Best Server Default")
                .WithDatesInterval(DateTime.Parse("2021-05-29Z"), DateTime.Parse("2021-05-31Z"))
                .WithContent(BestServerConfigurations.BestServerEvaluation, "Default")
                .Build();

            // Assert 3
            ruleBuilderResultDefault.Should().NotBeNull();
            errors = ruleBuilderResultDefault.Errors.Any() ? ruleBuilderResultDefault.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResultDefault.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 4 - Add new default rule
            addRuleResult = await rulesEngine.AddRuleAsync(ruleBuilderResultDefault.Rule, RuleAddPriorityOption.AtBottom).ConfigureAwait(false);

            // Assert 4 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 5 - Evaluate new rule with "in" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new[]
            {
                new Condition<BestServerConditions>(BestServerConditions.Price, 100),
                new Condition<BestServerConditions>(BestServerConditions.Memory, 12),
                new Condition<BestServerConditions>(BestServerConditions.StoragePartionable, true),
                new Condition<BestServerConditions>(BestServerConditions.Brand, "AMD")
            };

            var actual = await rulesEngine.MatchOneAsync(BestServerConfigurations.BestServerEvaluation, matchDateTime, conditions).ConfigureAwait(false);

            // Assert 5
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(rule);

            // Act 6 - Update Best Server Top5 rule deactivate
            var updateRuleResult = await rulesEngine.DeactivateRuleAsync(rule).ConfigureAwait(false);

            // Assert 6
            updateRuleResult.Should().NotBeNull();
            updateRuleResult.IsSuccess.Should().BeTrue();

            // Act 7 - Evaluate rule should be default now
            actual = await rulesEngine.MatchOneAsync(BestServerConfigurations.BestServerEvaluation, matchDateTime, conditions).ConfigureAwait(false);

            // Assert 7
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(ruleBuilderResultDefault.Rule);
        }

        [Fact]
        public async Task BestServer_UpdatingBestServerTop5_ReturnsBestServerDefault()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource<BestServerConfigurations, BestServerConditions>(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                 .WithContentType<BestServerConfigurations>()
                 .WithConditionType<BestServerConditions>()
                 .SetInMemoryDataSource(serviceProvider)
                 .Build();

            // Act 1 - Create rule with "in" operator
            var ruleBuilderResult = RuleBuilder.NewRule<BestServerConfigurations, BestServerConditions>()
                .WithName("Best Server Top5")
                .WithDatesInterval(DateTime.Parse("2021-05-29T11:00:00Z"), DateTime.Parse("2021-05-31Z"))
                .WithContent(BestServerConfigurations.BestServerEvaluation, "Top5")
                .WithCondition(c => c
                    .And(a => a
                        .Value(BestServerConditions.Price, Operators.In, new[] { 100m, 200m, 300m })
                        .Value(BestServerConditions.Memory, Operators.In, new[] { 12, 16, 24, 36 })
                        .Value(BestServerConditions.Memory, Operators.NotIn, new[] { 4, 8 })
                        .Value(BestServerConditions.StoragePartionable, Operators.In, new[] { true })
                        .Value(BestServerConditions.Brand, Operators.In, new[] { "AMD", "Intel", "Cisco" })
                        ))
                .Build();

            // Assert 1
            ruleBuilderResult.Should().NotBeNull();
            var errors = ruleBuilderResult.Errors.Any() ? ruleBuilderResult.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 2 - Add new rule with "in" operator
            var rule = ruleBuilderResult.Rule;

            var addRuleResult = await rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop).ConfigureAwait(false);

            // Assert 2 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 3 - Create rule default
            var ruleBuilderResultDefault = RuleBuilder.NewRule<BestServerConfigurations, BestServerConditions>()
                .WithName("Best Server Default")
                .WithDatesInterval(DateTime.Parse("2021-05-29Z"), DateTime.Parse("2021-05-31Z"))
                .WithContent(BestServerConfigurations.BestServerEvaluation, "Default")
                .Build();

            // Assert 3
            ruleBuilderResultDefault.Should().NotBeNull();
            errors = ruleBuilderResultDefault.Errors.Any() ? ruleBuilderResultDefault.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResultDefault.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 4 - Add new default rule
            addRuleResult = await rulesEngine.AddRuleAsync(ruleBuilderResultDefault.Rule, RuleAddPriorityOption.AtBottom).ConfigureAwait(false);

            // Assert 4 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 5 - Evaluate new rule with "in" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new[]
            {
                new Condition<BestServerConditions>(BestServerConditions.Price,100),
                new Condition<BestServerConditions>(BestServerConditions.Memory,12),
                new Condition<BestServerConditions>(BestServerConditions.StoragePartionable,true),
                new Condition<BestServerConditions>(BestServerConditions.Brand,"AMD")
            };

            var actual = await rulesEngine.MatchOneAsync(BestServerConfigurations.BestServerEvaluation, matchDateTime, conditions).ConfigureAwait(false);

            // Assert 5
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(rule);

            // Act 6 - Update Best Server Top5 date end
            rule.DateEnd = DateTime.Parse("2021-05-29T12:10:00Z");
            var updateRuleResult = await rulesEngine.UpdateRuleAsync(rule).ConfigureAwait(false);

            // Assert 6
            updateRuleResult.Should().NotBeNull();
            updateRuleResult.IsSuccess.Should().BeTrue();

            // Act 7 - Evaluate rule should be default now
            actual = await rulesEngine.MatchOneAsync(BestServerConfigurations.BestServerEvaluation, matchDateTime, conditions).ConfigureAwait(false);

            // Assert 7
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(ruleBuilderResultDefault.Rule);
        }
    }
}