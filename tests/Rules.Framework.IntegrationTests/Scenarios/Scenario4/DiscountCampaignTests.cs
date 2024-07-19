namespace Rules.Framework.IntegrationTests.Scenarios.Scenario4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Rules.Framework;
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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task DiscountsWeekend_Adding15PercentRulePerBrandAndEvaluatingOneOfTheBrands_Returns15PercentDiscountRate(bool enableCompilation)
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<DiscountConfigurations, DiscountConditions>();

            // Act 1 - Create rule with "in" operator
            var ruleBuilderResult = Rule.New<DiscountConfigurations, DiscountConditions>()
                .WithName("Discounts Weekend MAY2021")
                .WithDatesInterval(DateTime.Parse("2021-05-29Z"), DateTime.Parse("2021-05-31Z"))
                .WithContent(DiscountConfigurations.DiscountCampaigns, 15m)
                .WithCondition(c => c
                    .And(a => a
                        .Value(DiscountConditions.ProductRecommendedRetailPrice, Operators.GreaterThanOrEqual, 1000)
                        .Value(DiscountConditions.ProductBrand, Operators.In, new[] { "ASUS", "HP", "Dell", "Toshiba", "Acer" })))
                .Build();

            // Assert 1
            ruleBuilderResult.Should().NotBeNull();
            var errors = ruleBuilderResult.Errors.Any() ? ruleBuilderResult.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 2 - Add new rule with "in" operator
            var rule = ruleBuilderResult.Rule;

            var addRuleResult = await genericRulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop);

            // Assert 2 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 3 - Evaluate new rule with "in" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new[]
            {
                new Condition<DiscountConditions>(DiscountConditions.ProductBrand,"ASUS"),
                new Condition<DiscountConditions>(DiscountConditions.ProductRecommendedRetailPrice,1249.90m)
            };

            var actual = await genericRulesEngine.MatchOneAsync(DiscountConfigurations.DiscountCampaigns, matchDateTime, conditions);

            // Assert 3
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(rule, opt => opt.Excluding(r => r.RootCondition.Properties));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task DiscountsWeekend_Adding20PercentRulePerProductTierAndEvaluatingOneOfTheTiers_Returns20PercentDiscountRate(bool enableCompilation)
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<DiscountConfigurations, DiscountConditions>();

            // Act 1 - Create rule with "in" operator
            var ruleBuilderResult = Rule.New<DiscountConfigurations, DiscountConditions>()
                .WithName("Discounts Weekend MAY2021 - Tiered discount")
                .WithDatesInterval(DateTime.Parse("2021-05-29Z"), DateTime.Parse("2021-05-31Z"))
                .WithContent(DiscountConfigurations.DiscountCampaigns, 15m)
                .WithCondition(c => c
                    .And(a => a
                        .Value(DiscountConditions.ProductRecommendedRetailPrice, Operators.GreaterThanOrEqual, 1000)
                        .Value(DiscountConditions.ProductTier, Operators.In, new[] { 1, 3 })))
                .Build();

            // Assert 1
            ruleBuilderResult.Should().NotBeNull();
            var errors = ruleBuilderResult.Errors.Any() ? ruleBuilderResult.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 2 - Add new rule with "in" operator
            var rule = ruleBuilderResult.Rule;

            var addRuleResult = await genericRulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop);

            // Assert 2 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 3 - Evaluate new rule with "in" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new[]
            {
                new Condition<DiscountConditions>(DiscountConditions.ProductBrand, "ASUS"),
                new Condition<DiscountConditions>(DiscountConditions.ProductTier, 1),
                new Condition<DiscountConditions>(DiscountConditions.ProductRecommendedRetailPrice, 1249.90m)
            };

            var actual = await genericRulesEngine.MatchOneAsync(DiscountConfigurations.DiscountCampaigns, matchDateTime, conditions);

            // Assert 3
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(rule, opt => opt.Excluding(r => r.RootCondition.Properties));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task DiscountsWeekend_Adding5PercentRuleWithNotContainsTestConditionAndInputWithMatchingConditions_Return5PercentDiscountRate(bool enableCompilation)
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<DiscountConfigurations, DiscountConditions>();

            // Act 1 - Create rule with "not contains" operator
            var ruleBuilderResult = Rule.New<DiscountConfigurations, DiscountConditions>()
                    .WithName("Not a staff discount")
                    .WithContent(DiscountConfigurations.DiscountCampaigns, 5m)
                    .WithDateBegin(DateTime.Parse("2021-05-29Z"))
                    .WithCondition(DiscountConditions.CustomerEmail, Operators.NotContains, "@staff.com")
                    .Build();

            // Assert 1
            ruleBuilderResult.Should().NotBeNull();
            var errors = ruleBuilderResult.Errors.Any() ? ruleBuilderResult.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 2 - Add new rule with "not contains" operator
            var rule = ruleBuilderResult.Rule;

            var addRuleResult = await genericRulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop);

            // Assert 2 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 3 - Evaluate new rule with "not contains" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new[]
            {
                new Condition<DiscountConditions>(DiscountConditions.CustomerEmail, "user12345@somewhere.com")
            };

            var actual = await genericRulesEngine.MatchOneAsync(DiscountConfigurations.DiscountCampaigns, matchDateTime, conditions);

            // Assert 3
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(rule, opt => opt.Excluding(r => r.RootCondition.Properties));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task DiscountsWeekend_AddingRuleWithNullTestConditionAndInputWithMatchingConditions_ReturnNotNull(bool enableCompilation)
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<DiscountConfigurations, DiscountConditions>();

            // Act 1 - Create rule with "equal" operator
            var ruleBuilderResult = Rule.New<DiscountConfigurations, DiscountConditions>()
                    .WithName("Blue Product")
                    .WithContent(DiscountConfigurations.DiscountCampaigns, ProductColor.Blue.ToString())
                    .WithDateBegin(DateTime.Parse("2021-05-29Z"))
                    .WithCondition(DiscountConditions.ProductColor, Operators.Equal, ProductColor.Blue.ToString())
                    .Build();

            // Assert 1
            ruleBuilderResult.Should().NotBeNull();
            var errors = ruleBuilderResult.Errors.Any() ? ruleBuilderResult.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 2 - Add new rule with "in" operator
            var rule = ruleBuilderResult.Rule;

            var addRuleResult = await genericRulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop);

            // Assert 2 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 3 - Evaluate new rule with "in" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new[]
            {
                new Condition<DiscountConditions>(DiscountConditions.ProductColor, ProductColor.Blue.ToString())
            };

            var actual = await genericRulesEngine.MatchOneAsync(DiscountConfigurations.DiscountCampaigns, matchDateTime, conditions);

            // Assert 3
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(rule, opt => opt.Excluding(r => r.RootCondition.Properties));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task DiscountsWeekend_AddingRuleWithNullTestConditionAndInputWithoutConditions_ReturnNull(bool enableCompilation)
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<DiscountConfigurations, DiscountConditions>();

            // Act 1 - Create rule with "equal" operator
            var ruleBuilderResult = Rule.New<DiscountConfigurations, DiscountConditions>()
                    .WithName("Blue Product")
                    .WithContent(DiscountConfigurations.DiscountCampaigns, ProductColor.Blue.ToString())
                    .WithDateBegin(DateTime.Parse("2021-05-29Z"))
                    .WithCondition(DiscountConditions.ProductColor, Operators.Equal, ProductColor.Blue.ToString())
                    .Build();

            // Assert 1
            ruleBuilderResult.Should().NotBeNull();
            var errors = ruleBuilderResult.Errors.Any() ? ruleBuilderResult.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 2 - Add new rule with "in" operator
            var rule = ruleBuilderResult.Rule;

            var addRuleResult = await genericRulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop);

            // Assert 2 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 3 - Evaluate new rule with "in" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new List<Condition<DiscountConditions>>();

            var actual = await genericRulesEngine.MatchOneAsync(DiscountConfigurations.DiscountCampaigns, matchDateTime, conditions);

            // Assert 3
            actual.Should().BeNull();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task DiscountsWeekend_AddingRuleWithNullTestConditionAndInputWithoutMatchingConditions_ReturnNull(bool enableCompilation)
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource(serviceProvider)
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<DiscountConfigurations, DiscountConditions>();

            // Act 1 - Create rule with "equal" operator
            var ruleBuilderResult = Rule.New<DiscountConfigurations, DiscountConditions>()
                    .WithName("Blue Product")
                    .WithContent(DiscountConfigurations.DiscountCampaigns, ProductColor.Blue.ToString())
                    .WithDateBegin(DateTime.Parse("2021-05-29Z"))
                    .WithCondition(DiscountConditions.ProductColor, Operators.Equal, ProductColor.Blue.ToString())
                    .Build();

            // Assert 1
            ruleBuilderResult.Should().NotBeNull();
            var errors = ruleBuilderResult.Errors.Any() ? ruleBuilderResult.Errors.Aggregate((s1, s2) => $"{s1}\n- {s2}") : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 2 - Add new rule with "in" operator
            var rule = ruleBuilderResult.Rule;

            var addRuleResult = await genericRulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtTop);

            // Assert 2 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 3 - Evaluate new rule with "in" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new[]
            {
                new Condition<DiscountConditions>(DiscountConditions.ProductColor, ProductColor.White.ToString())
            };

            var actual = await genericRulesEngine.MatchOneAsync(DiscountConfigurations.DiscountCampaigns, matchDateTime, conditions);

            // Assert 3
            actual.Should().BeNull();
        }
    }
}