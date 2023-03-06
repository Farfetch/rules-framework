namespace Rules.Framework.IntegrationTests.Tests.Scenario4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario4;
    using Xunit;

    public class DiscountCampaignTests
    {
        public enum ProductColor
        {
            Blue = 0,
            Red = 1,
            White = 2
        }

        [Fact]
        public async Task DiscountsWeekend_Adding15PercentRulePerBrandAndEvaluatingOneOfTheBrands_Returns15PercentDiscountRate()
        {
            // Arrange
            var rulesDataSource
                = new InMemoryRulesDataSource<DiscountConfigurations, DiscountConditions>(Enumerable.Empty<Rule<DiscountConfigurations, DiscountConditions>>());

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<DiscountConfigurations>()
                .WithConditionType<DiscountConditions>()
                .SetDataSource(rulesDataSource)
                .Build();

            // Act 1 - Create rule with "in" operator
            var ruleBuilderResult = RuleBuilder.NewRule<DiscountConfigurations, DiscountConditions>()
                .WithName("Discounts Weekend MAY2021")
                .WithDatesInterval(DateTime.Parse("2021-05-29Z"), DateTime.Parse("2021-05-31Z"))
                .WithContentContainer(new ContentContainer<DiscountConfigurations>(DiscountConfigurations.DiscountCampaigns, t => 15m))
                .WithCondition(cnb =>
                    cnb.AsComposed()
                        .WithLogicalOperator(LogicalOperators.And)
                        .AddCondition(x1 =>
                            x1.AsValued(DiscountConditions.ProductRecommendedRetailPrice)
                                .OfDataType<decimal>()
                                .WithComparisonOperator(Operators.GreaterThanOrEqual)
                                .SetOperand(1000)
                                .Build())
                        .AddCondition(x2 =>
                            x2.AsValued(DiscountConditions.ProductBrand)
                            .OfDataType<string>()
                            .WithComparisonOperator(Operators.In)
                            .SetOperand(new[] { "ASUS", "HP", "Dell", "Toshiba", "Acer" })
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

            // Act 3 - Evaluate new rule with "in" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new[]
            {
                new Condition<DiscountConditions>
                {
                    Type = DiscountConditions.ProductBrand,
                    Value = "ASUS"
                },
                new Condition<DiscountConditions>
                {
                    Type = DiscountConditions.ProductRecommendedRetailPrice,
                    Value = 1249.90m
                }
            };

            var actual = await rulesEngine.MatchOneAsync(DiscountConfigurations.DiscountCampaigns, matchDateTime, conditions).ConfigureAwait(false);

            // Assert 3
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(rule);
        }

        [Fact]
        public async Task DiscountsWeekend_Adding20PercentRulePerProductTierAndEvaluatingOneOfTheTiers_Returns20PercentDiscountRate()
        {
            // Arrange
            var rulesDataSource
                = new InMemoryRulesDataSource<DiscountConfigurations, DiscountConditions>(Enumerable.Empty<Rule<DiscountConfigurations, DiscountConditions>>());

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<DiscountConfigurations>()
                .WithConditionType<DiscountConditions>()
                .SetDataSource(rulesDataSource)
                .Build();

            // Act 1 - Create rule with "in" operator
            var ruleBuilderResult = RuleBuilder.NewRule<DiscountConfigurations, DiscountConditions>()
                .WithName("Discounts Weekend MAY2021 - Tiered discount")
                .WithDatesInterval(DateTime.Parse("2021-05-29Z"), DateTime.Parse("2021-05-31Z"))
                .WithContentContainer(new ContentContainer<DiscountConfigurations>(DiscountConfigurations.DiscountCampaigns, t => 15m))
                .WithCondition(cnb =>
                    cnb.AsComposed()
                        .WithLogicalOperator(LogicalOperators.And)
                        .AddCondition(x1 =>
                            x1.AsValued(DiscountConditions.ProductRecommendedRetailPrice)
                                .OfDataType<decimal>()
                                .WithComparisonOperator(Operators.GreaterThanOrEqual)
                                .SetOperand(1000)
                                .Build())
                        .AddCondition(x2 =>
                            x2.AsValued(DiscountConditions.ProductTier)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.In)
                            .SetOperand(new[] { 1, 3 })
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

            // Act 3 - Evaluate new rule with "in" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new[]
            {
                new Condition<DiscountConditions>
                {
                    Type = DiscountConditions.ProductBrand,
                    Value = "ASUS"
                },
                new Condition<DiscountConditions>
                {
                    Type = DiscountConditions.ProductTier,
                    Value = 1
                },
                new Condition<DiscountConditions>
                {
                    Type = DiscountConditions.ProductRecommendedRetailPrice,
                    Value = 1249.90m
                }
            };

            var actual = await rulesEngine.MatchOneAsync(DiscountConfigurations.DiscountCampaigns, matchDateTime, conditions).ConfigureAwait(false);

            // Assert 3
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(rule);
        }

        [Fact]
        public async Task DiscountsWeekend_Adding5PercentRuleWithNotContainsTestConditionAndInputWithMatchingConditions_Return5PercentDiscountRate()
        {
            // Arrange
            var rulesDataSource
                = new InMemoryRulesDataSource<DiscountConfigurations, DiscountConditions>(Enumerable.Empty<Rule<DiscountConfigurations, DiscountConditions>>());

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<DiscountConfigurations>()
                .WithConditionType<DiscountConditions>()
                .SetDataSource(rulesDataSource)
                .Build();

            // Act 1 - Create rule with "not contains" operator
            var ruleBuilderResult =
                RuleBuilder
                    .NewRule<DiscountConfigurations, DiscountConditions>()
                    .WithName("Not a staff discount")
                    .WithContentContainer(new ContentContainer<DiscountConfigurations>(DiscountConfigurations.DiscountCampaigns, t => 5m))
                    .WithDateBegin(DateTime.Parse("2021-05-29Z"))
                    .WithCondition(x1 =>
                                x1.AsValued(DiscountConditions.CustomerEmail)
                                    .OfDataType<string>()
                                    .WithComparisonOperator(Operators.NotContains)
                                    .SetOperand("@staff.com")
                                    .Build()
                    ).Build();

            // Assert 1
            ruleBuilderResult.Should().NotBeNull();
            var errors = ruleBuilderResult.Errors.Any() ? ruleBuilderResult.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 2 - Add new rule with "not contains" operator
            var rule = ruleBuilderResult.Rule;

            var addRuleResult = await rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop).ConfigureAwait(false);

            // Assert 2 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 3 - Evaluate new rule with "not contains" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new[]
            {
                new Condition<DiscountConditions>
                {
                    Type = DiscountConditions.CustomerEmail,
                    Value = "user12345@somewhere.com"
                }
            };

            var actual = await rulesEngine.MatchOneAsync(DiscountConfigurations.DiscountCampaigns, matchDateTime, conditions).ConfigureAwait(false);

            // Assert 3
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(rule);
        }

        [Fact]
        public async Task DiscountsWeekend_AddingRuleWithNullTestConditionAndInputWithMatchingConditions_ReturnNotNull()
        {
            // Arrange
            var rulesDataSource
                = new InMemoryRulesDataSource<DiscountConfigurations, DiscountConditions>(Enumerable.Empty<Rule<DiscountConfigurations, DiscountConditions>>());

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<DiscountConfigurations>()
                .WithConditionType<DiscountConditions>()
                .SetDataSource(rulesDataSource)
                .Build();

            // Act 1 - Create rule with "equal" operator
            var ruleBuilderResult =
                RuleBuilder
                    .NewRule<DiscountConfigurations, DiscountConditions>()
                    .WithName("Blue Product")
                    .WithContentContainer(new ContentContainer<DiscountConfigurations>(DiscountConfigurations.DiscountCampaigns, t => ProductColor.Blue.ToString()))
                    .WithDateBegin(DateTime.Parse("2021-05-29Z"))
                    .WithCondition(x1 =>
                                x1.AsValued(DiscountConditions.ProductColor)
                                    .OfDataType<string>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(ProductColor.Blue.ToString())
                                    .Build()
                    ).Build();

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

            // Act 3 - Evaluate new rule with "in" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new[]
            {
                new Condition<DiscountConditions>
                {
                    Type = DiscountConditions.ProductColor,
                    Value = ProductColor.Blue.ToString()
                }
            };

            var actual = await rulesEngine.MatchOneAsync(DiscountConfigurations.DiscountCampaigns, matchDateTime, conditions).ConfigureAwait(false);

            // Assert 3
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(rule);
        }

        [Fact]
        public async Task DiscountsWeekend_AddingRuleWithNullTestConditionAndInputWithoutConditions_ReturnNull()
        {
            // Arrange
            var rulesDataSource
                = new InMemoryRulesDataSource<DiscountConfigurations, DiscountConditions>(Enumerable.Empty<Rule<DiscountConfigurations, DiscountConditions>>());

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<DiscountConfigurations>()
                .WithConditionType<DiscountConditions>()
                .SetDataSource(rulesDataSource)
                .Build();

            // Act 1 - Create rule with "equal" operator
            var ruleBuilderResult =
                RuleBuilder
                    .NewRule<DiscountConfigurations, DiscountConditions>()
                    .WithName("Blue Product")
                    .WithContentContainer(new ContentContainer<DiscountConfigurations>(DiscountConfigurations.DiscountCampaigns, t => ProductColor.Blue.ToString()))
                    .WithDateBegin(DateTime.Parse("2021-05-29Z"))
                    .WithCondition(x1 =>
                                x1.AsValued(DiscountConditions.ProductColor)
                                    .OfDataType<string>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(ProductColor.Blue.ToString())
                                    .Build()
                    ).Build();

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

            // Act 3 - Evaluate new rule with "in" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new List<Condition<DiscountConditions>>();

            var actual = await rulesEngine.MatchOneAsync(DiscountConfigurations.DiscountCampaigns, matchDateTime, conditions).ConfigureAwait(false);

            // Assert 3
            actual.Should().BeNull();
        }

        [Fact]
        public async Task DiscountsWeekend_AddingRuleWithNullTestConditionAndInputWithoutMatchingConditions_ReturnNull()
        {
            // Arrange
            var rulesDataSource
                = new InMemoryRulesDataSource<DiscountConfigurations, DiscountConditions>(Enumerable.Empty<Rule<DiscountConfigurations, DiscountConditions>>());

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<DiscountConfigurations>()
                .WithConditionType<DiscountConditions>()
                .SetDataSource(rulesDataSource)
                .Build();

            // Act 1 - Create rule with "equal" operator
            var ruleBuilderResult =
                RuleBuilder
                    .NewRule<DiscountConfigurations, DiscountConditions>()
                    .WithName("Blue Product")
                    .WithContentContainer(new ContentContainer<DiscountConfigurations>(DiscountConfigurations.DiscountCampaigns, t => ProductColor.Blue.ToString()))
                    .WithDateBegin(DateTime.Parse("2021-05-29Z"))
                    .WithCondition(x1 =>
                                x1.AsValued(DiscountConditions.ProductColor)
                                    .OfDataType<string>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(ProductColor.Blue.ToString())
                                    .Build()
                    ).Build();

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

            // Act 3 - Evaluate new rule with "in" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new[]
            {
                new Condition<DiscountConditions>
                {
                    Type = DiscountConditions.ProductColor,
                    Value = ProductColor.White.ToString()
                }
            };

            var actual = await rulesEngine.MatchOneAsync(DiscountConfigurations.DiscountCampaigns, matchDateTime, conditions).ConfigureAwait(false);

            // Assert 3
            actual.Should().BeNull();
        }
    }
}