namespace Rules.Framework.IntegrationTests.Scenarios.Scenario1
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Rules.Framework;
    using Rules.Framework.IntegrationTests.Common.Scenarios.Scenario1;
    using Rules.Framework.Providers.InMemory;
    using Xunit;

    public class BodyMassIndexTests
    {
        private static string DataSourceFilePath => $@"{Environment.CurrentDirectory}/Scenarios/Scenario1/rules-framework-tests.body-mass-index.datasource.json";

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task AddRule_AddingNewRuleFromScratchWithAgeConditionAtPriority1AndNewRuleAtPriority3_NewRuleIsInsertedAndExistentRulePriorityUpdatedAndNewRuleInsertedAfter(bool enableCompilation)
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
            var genericRulesEngine = rulesEngine.MakeGeneric<ContentTypes, ConditionTypes>();

            var newRuleResult1 = Rule.New<ContentTypes, ConditionTypes>()
                .WithName("Body Mass Index up to 18 years formula")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContent(ContentTypes.BodyMassIndexFormula, new Formula
                {
                    Description = "Body Mass Index up to 18 years formula",
                    Value = "weight / ((height + 1) ^ 2)" // Not real, for the sake of the test.
                })
                .WithCondition(ConditionTypes.Age, Operators.LesserThanOrEqual, 18)
                .Build();

            var newRule1 = newRuleResult1.Rule;
            var ruleAddPriorityOption1 = RuleAddPriorityOption.ByPriorityNumber(1);

            var ruleBuilderResult2 = Rule.New<ContentTypes, ConditionTypes>()
                .WithName("Sample rule")
                .WithDateBegin(DateTime.Parse("2021-01-01"))
                .WithContent(ContentTypes.BodyMassIndexFormula, new Formula
                {
                    Description = "Sample formula",
                    Value = "0"
                })
                .Build();

            var newRule2 = ruleBuilderResult2.Rule;
            var ruleAddPriorityOption2 = RuleAddPriorityOption.ByPriorityNumber(4);

            // Act
            var ruleOperationResult1 = await genericRulesEngine.AddRuleAsync(newRule1, ruleAddPriorityOption1);
            var ruleOperationResult2 = await genericRulesEngine.AddRuleAsync(newRule2, ruleAddPriorityOption2);

            // Assert
            ruleOperationResult1.Should().NotBeNull();
            ruleOperationResult1.IsSuccess.Should().BeTrue();

            ruleOperationResult2.Should().NotBeNull();
            ruleOperationResult2.IsSuccess.Should().BeTrue();

            var inMemoryRulesStorage = serviceProvider.GetService<IInMemoryRulesStorage>();
            var rulesDataSource = CreateRulesDataSource(inMemoryRulesStorage);
            var rules = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs());
            rules.Should().NotBeNull().And.HaveCount(2);
            rules.Should().ContainEquivalentOf((Rule)newRule1);
            newRule1.Priority.Should().Be(1, "rule should to priority 1 if inserted at priority 1");
            newRule2.Priority.Should().Be(2, "rule should have priority 2 if inserted at priority 2, given that last rule after insert was at priority 2.");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task AddRule_AddingNewRuleWithAgeConditionAtPriority1AndNewRuleAtPriority3_NewRuleIsInsertedAndExistentRulePriorityUpdatedAndNewRuleInsertedAfter(bool enableCompilation)
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
            var genericRulesEngine = rulesEngine.MakeGeneric<ContentTypes, ConditionTypes>();

            await RulesFromJsonFile.Load
                .FromJsonFileAsync(genericRulesEngine, DataSourceFilePath, typeof(Formula));

            var newRuleResult1 = Rule.New<ContentTypes, ConditionTypes>()
                .WithName("Body Mass Index up to 18 years formula")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContent(ContentTypes.BodyMassIndexFormula, new Formula
                {
                    Description = "Body Mass Index up to 18 years formula",
                    Value = "weight / ((height + 1) ^ 2)" // Not real, for the sake of the test.
                })
                .WithCondition(ConditionTypes.Age, Operators.LesserThanOrEqual, 18)
                .Build();

            var newRule1 = newRuleResult1.Rule;
            var ruleAddPriorityOption1 = RuleAddPriorityOption.ByPriorityNumber(1);

            var ruleBuilderResult2 = Rule.New<ContentTypes, ConditionTypes>()
                .WithName("Sample rule")
                .WithDateBegin(DateTime.Parse("2021-01-01"))
                .WithContent(ContentTypes.BodyMassIndexFormula, new Formula
                {
                    Description = "Sample formula",
                    Value = "0"
                })
                .Build();

            var newRule2 = ruleBuilderResult2.Rule;
            var ruleAddPriorityOption2 = RuleAddPriorityOption.ByPriorityNumber(4);

            // Act
            var ruleOperationResult1 = await genericRulesEngine.AddRuleAsync(newRule1, ruleAddPriorityOption1);
            var ruleOperationResult2 = await genericRulesEngine.AddRuleAsync(newRule2, ruleAddPriorityOption2);

            // Assert
            ruleOperationResult1.Should().NotBeNull();
            ruleOperationResult1.IsSuccess.Should().BeTrue();

            ruleOperationResult2.Should().NotBeNull();
            ruleOperationResult2.IsSuccess.Should().BeTrue();

            var inMemoryRulesStorage = serviceProvider.GetService<IInMemoryRulesStorage>();
            var rulesDataSource = CreateRulesDataSource(inMemoryRulesStorage);
            var rules = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs());
            rules.Should().NotBeNull().And.HaveCount(3);
            rules.Should().ContainEquivalentOf((Rule)newRule1);
            newRule1.Priority.Should().Be(1, "rule should to priority 1 if inserted at priority 1");
            newRule2.Priority.Should().Be(3, "rule should have priority 3 if inserted at priority 3, given that last rule after insert was at priority 2.");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task AddRule_AddingNewRuleWithAgeConditionOnTop_NewRuleIsInsertedAndExistentRulePriorityUpdated(bool enableCompilation)
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
            var genericRulesEngine = rulesEngine.MakeGeneric<ContentTypes, ConditionTypes>();

            await RulesFromJsonFile.Load
                .FromJsonFileAsync(genericRulesEngine, DataSourceFilePath, typeof(Formula));

            var newRuleResult = Rule.New<ContentTypes, ConditionTypes>()
                .WithName("Body Mass Index up to 18 years formula")
                .WithDateBegin(DateTime.Parse("2018-01-01"))
                .WithContent(ContentTypes.BodyMassIndexFormula, new Formula
                {
                    Description = "Body Mass Index up to 18 years formula",
                    Value = "weight / ((height + 1) ^ 2)" // Not real, for the sake of the test.
                })
                .WithCondition(ConditionTypes.Age, Operators.LesserThanOrEqual, 18)
                .Build();

            var newRule = newRuleResult.Rule;
            var ruleAddPriorityOption = new RuleAddPriorityOption
            {
                PriorityOption = PriorityOptions.AtTop
            };

            // Act
            var ruleOperationResult = await genericRulesEngine.AddRuleAsync(newRule, ruleAddPriorityOption);

            // Assert
            ruleOperationResult.Should().NotBeNull();
            ruleOperationResult.IsSuccess.Should().BeTrue();

            var inMemoryRulesStorage = serviceProvider.GetService<IInMemoryRulesStorage>();
            var rulesDataSource = CreateRulesDataSource(inMemoryRulesStorage);
            var rules = await rulesDataSource.GetRulesByAsync(new RulesFilterArgs());
            rules.Should().NotBeNull().And.HaveCount(2);
            rules.Should().ContainEquivalentOf((Rule)newRule, o => o.Excluding(x => x.RootCondition.Properties));
            newRule.Priority.Should().Be(1, "rule should to priority 1 if inserted at top.");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task BodyMassIndex_NoConditions_ReturnsDefaultFormula(bool enableCompilation)
        {
            // Arrange
            var expectedFormulaDescription = "Body Mass Index default formula";
            var expectedFormulaValue = "weight / (height ^ 2)";
            const ContentTypes expectedContent = ContentTypes.BodyMassIndexFormula;
            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new Condition<ConditionTypes>[0];

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
            var genericRulesEngine = rulesEngine.MakeGeneric<ContentTypes, ConditionTypes>();

            await RulesFromJsonFile.Load
                .FromJsonFileAsync(genericRulesEngine, DataSourceFilePath, typeof(Formula));

            // Act
            var actual = await genericRulesEngine.MatchOneAsync(expectedContent, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();
            var actualFormula = actual.ContentContainer.GetContentAs<Formula>();
            actualFormula.Description.Should().Be(expectedFormulaDescription);
            actualFormula.Value.Should().Be(expectedFormulaValue);
        }

        private static InMemoryProviderRulesDataSource CreateRulesDataSource(IInMemoryRulesStorage inMemoryRulesStorage)
        {
            var ruleFactory = new RuleFactory();
            return new InMemoryProviderRulesDataSource(inMemoryRulesStorage, ruleFactory);
        }
    }
}