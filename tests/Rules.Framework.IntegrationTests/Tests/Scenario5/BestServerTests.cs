namespace Rules.Framework.IntegrationTests.Tests.Scenario5
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario5;
    using Rules.Framework.Providers.InMemory;
    using Xunit;

    public class BestServerTests
    {
        public readonly static IEnumerable<object[]> DataTest = new List<object[]>
        {
            new object[]
            {
                new[]
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
                },
                "Best Server Top5"
            },
            new object[]
            {
                new[]
                {
                    new Condition<BestServerConditions>
                    {
                        Type = BestServerConditions.Price,
                        Value = 110
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
                },
                "Best Server Default"
            }
        };

        [Theory]
        [MemberData(nameof(DataTest))]
        public async Task BestServer_InEvaluation(IEnumerable<Condition<BestServerConditions>> conditions, string expectedRuleName)
        {
            // Arrange
            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<BestServerConfigurations>()
                .WithConditionType<BestServerConditions>()
                .SetInMemoryDataSource()
                .Build();

            // Act 1 - Create rule with "in" operator
            var ruleBuilderResult = RuleBuilder.NewRule<BestServerConfigurations, BestServerConditions>()
                .WithName("Best Server Top5")
                .WithDatesInterval(DateTime.Parse("2021-05-29Z"), DateTime.Parse("2021-05-31Z"))
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

            // Act 2 - Create rule default
            var ruleBuilderResultDefault = RuleBuilder.NewRule<BestServerConfigurations, BestServerConditions>()
                .WithName("Best Server Default")
                .WithDatesInterval(DateTime.Parse("2021-05-29Z"), DateTime.Parse("2021-05-31Z"))
                 .WithContentContainer(new ContentContainer<BestServerConfigurations>(BestServerConfigurations.BestServerEvaluation, t => "Default"))
                .Build();

            // Assert 1
            ruleBuilderResult.Should().NotBeNull();
            string errors = ruleBuilderResult.Errors.Any() ? ruleBuilderResult.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Assert 3
            ruleBuilderResultDefault.Should().NotBeNull();
            errors = ruleBuilderResultDefault.Errors.Any() ? ruleBuilderResultDefault.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResultDefault.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule default: \n[\n- {errors}\n]");

            // Act 2 - Add new rule with "in" operator
            await rulesEngine.AddRuleAsync(ruleBuilderResultDefault.Rule, RuleAddPriorityOption.ByPriorityNumber(2)).ConfigureAwait(false);
            await rulesEngine.AddRuleAsync(ruleBuilderResult.Rule, RuleAddPriorityOption.ByPriorityNumber(1)).ConfigureAwait(false);

            DateTime matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");

            var actual = await rulesEngine.MatchOneAsync(BestServerConfigurations.BestServerEvaluation, matchDateTime, conditions).ConfigureAwait(false);

            // Assert 3
            actual.Should().NotBeNull();
            actual.Name.Should().BeEquivalentTo(expectedRuleName);
        }
    }
}