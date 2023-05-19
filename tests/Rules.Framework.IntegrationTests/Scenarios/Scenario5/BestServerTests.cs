namespace Rules.Framework.IntegrationTests.Scenarios.Scenario5
{
    using System;
    using System.Collections.Generic;
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
                .WithContentContainer(new ContentContainer<BestServerConfigurations>(BestServerConfigurations.BestServerEvaluation, t => "Top5"))
                .WithCondition(cnb =>
                    cnb.AsComposed()
                        .WithLogicalOperator(LogicalOperators.And)
                        .AddCondition(x1 =>
                            x1.AsValued(BestServerConditions.Price)
                                .OfDataType<IEnumerable<decimal>>()
                                .WithComparisonOperator(Operators.In)
                                .SetOperand(new[] { 100m, 200m, 300m })
                                .Build())
                        .AddCondition(x2 =>
                            x2.AsValued(BestServerConditions.Memory)
                            .OfDataType<IEnumerable<int>>()
                            .WithComparisonOperator(Operators.In)
                            .SetOperand(new[] { 12, 16, 24, 36 })
                            .Build())
                        .AddCondition(x2 =>
                            x2.AsValued(BestServerConditions.Memory)
                            .OfDataType<IEnumerable<int>>()
                            .WithComparisonOperator(Operators.NotIn)
                            .SetOperand(new[] { 4, 8 })
                            .Build())
                        .AddCondition(x2 =>
                            x2.AsValued(BestServerConditions.StoragePartionable)
                            .OfDataType<IEnumerable<bool>>()
                            .WithComparisonOperator(Operators.In)
                            .SetOperand(new[] { true })
                            .Build())
                        .AddCondition(x2 =>
                            x2.AsValued(BestServerConditions.Brand)
                            .OfDataType<IEnumerable<string>>()
                            .WithComparisonOperator(Operators.In)
                            .SetOperand(new[] { "AMD", "Intel", "Cisco" })
                            .Build())
                        .Build())

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
                 .WithContentContainer(new ContentContainer<BestServerConfigurations>(BestServerConfigurations.BestServerEvaluation, t => "Default"))
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
                new Condition<BestServerConditions>
                    {
                        Type = BestServerConditions.Price,
                        Value = 100
                    },
                    new Condition<BestServerConditions>
                    {
                        Type = BestServerConditions.Memory,
                        Value = 12
                    },
                    new Condition<BestServerConditions>
                    {
                        Type = BestServerConditions.StoragePartionable,
                        Value = true
                    },
                    new Condition<BestServerConditions>
                    {
                        Type = BestServerConditions.Brand,
                        Value = "AMD"
                    }
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
                .WithContentContainer(new ContentContainer<BestServerConfigurations>(BestServerConfigurations.BestServerEvaluation, t => "Top5"))
                .WithCondition(cnb =>
                    cnb.AsComposed()
                        .WithLogicalOperator(LogicalOperators.And)
                        .AddCondition(x1 =>
                            x1.AsValued(BestServerConditions.Price)
                                .OfDataType<IEnumerable<decimal>>()
                                .WithComparisonOperator(Operators.In)
                                .SetOperand(new[] { 100m, 200m, 300m })
                                .Build())
                        .AddCondition(x2 =>
                            x2.AsValued(BestServerConditions.Memory)
                            .OfDataType<IEnumerable<int>>()
                            .WithComparisonOperator(Operators.In)
                            .SetOperand(new[] { 12, 16, 24, 36 })
                            .Build())
                        .AddCondition(x2 =>
                            x2.AsValued(BestServerConditions.Memory)
                            .OfDataType<IEnumerable<int>>()
                            .WithComparisonOperator(Operators.NotIn)
                            .SetOperand(new[] { 4, 8 })
                            .Build())
                        .AddCondition(x2 =>
                            x2.AsValued(BestServerConditions.StoragePartionable)
                            .OfDataType<IEnumerable<bool>>()
                            .WithComparisonOperator(Operators.In)
                            .SetOperand(new[] { true })
                            .Build())
                        .AddCondition(x2 =>
                            x2.AsValued(BestServerConditions.Brand)
                            .OfDataType<IEnumerable<string>>()
                            .WithComparisonOperator(Operators.In)
                            .SetOperand(new[] { "AMD", "Intel", "Cisco" })
                            .Build())
                        .Build())

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
                 .WithContentContainer(new ContentContainer<BestServerConfigurations>(BestServerConfigurations.BestServerEvaluation, t => "Default"))
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
                new Condition<BestServerConditions>
                    {
                        Type = BestServerConditions.Price,
                        Value = 100
                    },
                    new Condition<BestServerConditions>
                    {
                        Type = BestServerConditions.Memory,
                        Value = 12
                    },
                    new Condition<BestServerConditions>
                    {
                        Type = BestServerConditions.StoragePartionable,
                        Value = true
                    },
                    new Condition<BestServerConditions>
                    {
                        Type = BestServerConditions.Brand,
                        Value = "AMD"
                    }
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