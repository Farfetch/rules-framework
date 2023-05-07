namespace Rules.Framework.IntegrationTests.Scenarios.Scenario8
{
    using System;
    using FluentAssertions;
    using Rules.Framework.BenchmarkTests.Tests.Benchmark3;
    using Rules.Framework.IntegrationTests.Common.Scenarios;
    using Rules.Framework.Providers.InMemory;
    using Xunit;

    public class TexasHoldEmPokerSingleCombinationsTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async void PokerCombinations_Given_EvaluatesStraightCombination(bool enableCompilation)
        {
            // Arrange
            var matchDate = new DateTime(2023, 1, 1);
            var conditions = new[]
            {
                new Condition<ConditionTypes> { Type = ConditionTypes.NumberOfKings, Value = 1 },
                new Condition<ConditionTypes> { Type = ConditionTypes.NumberOfQueens, Value = 1 },
                new Condition<ConditionTypes> { Type = ConditionTypes.NumberOfJacks, Value = 1 },
                new Condition<ConditionTypes> { Type = ConditionTypes.NumberOfTens, Value = 1 },
                new Condition<ConditionTypes> { Type = ConditionTypes.NumberOfNines, Value = 1 },
                new Condition<ConditionTypes> { Type = ConditionTypes.KingOfClubs, Value = true },
                new Condition<ConditionTypes> { Type = ConditionTypes.QueenOfDiamonds, Value = true },
                new Condition<ConditionTypes> { Type = ConditionTypes.JackOfClubs, Value = true },
                new Condition<ConditionTypes> { Type = ConditionTypes.TenOfHearts, Value = true },
                new Condition<ConditionTypes> { Type = ConditionTypes.NineOfSpades, Value = true },
            };

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .WithContentType<ContentTypes>()
                .WithConditionType<ConditionTypes>()
                .SetInMemoryDataSource()
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();

            var scenarioData = new Scenario8Data();

            await ScenarioLoader.LoadScenarioAsync(rulesEngine, scenarioData);

            // Act
            var result = await rulesEngine.MatchOneAsync(ContentTypes.TexasHoldemPokerSingleCombinations, matchDate, conditions);

            // Assert
            result.Should().NotBeNull();
            var resultContent = result.ContentContainer.GetContentAs<SingleCombinationPokerScore>();
            resultContent.Should().NotBeNull();
            resultContent.Combination.Should().Be("Straight");
        }
    }
}