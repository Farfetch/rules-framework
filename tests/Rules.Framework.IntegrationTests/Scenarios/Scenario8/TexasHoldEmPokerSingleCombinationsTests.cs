namespace Rules.Framework.IntegrationTests.Scenarios.Scenario8
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Rules.Framework.BenchmarkTests.Tests.Benchmark3;
    using Rules.Framework.IntegrationTests.Common.Scenarios;
    using Xunit;

    public class TexasHoldEmPokerSingleCombinationsTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task PokerCombinations_Given_EvaluatesStraightCombination(bool enableCompilation)
        {
            // Arrange
            var matchDate = new DateTime(2023, 1, 1);
            var conditions = new[]
            {
                new Condition<PokerConditions>(PokerConditions.NumberOfKings, 1),
                new Condition<PokerConditions>(PokerConditions.NumberOfQueens, 1),
                new Condition<PokerConditions>(PokerConditions.NumberOfJacks, 1 ),
                new Condition<PokerConditions>(PokerConditions.NumberOfTens, 1 ),
                new Condition<PokerConditions>(PokerConditions.NumberOfNines, 1),
                new Condition<PokerConditions>(PokerConditions.KingOfClubs, true),
                new Condition<PokerConditions>(PokerConditions.QueenOfDiamonds, true),
                new Condition<PokerConditions>(PokerConditions.JackOfClubs, true),
                new Condition<PokerConditions>(PokerConditions.TenOfHearts, true),
                new Condition<PokerConditions>(PokerConditions.NineOfSpades, true),
            };

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource()
                .Configure(options =>
                {
                    options.EnableCompilation = enableCompilation;
                })
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<PokerRulesets, PokerConditions>();

            var scenarioData = new Scenario8Data();

            await ScenarioLoader.LoadScenarioAsync(rulesEngine, scenarioData);

            // Act
            var result = await genericRulesEngine.MatchOneAsync(PokerRulesets.TexasHoldemPokerSingleCombinations, matchDate, conditions);

            // Assert
            result.Should().NotBeNull();
            var resultContent = result.ContentContainer.GetContentAs<SingleCombinationPokerScore>();
            resultContent.Should().NotBeNull();
            resultContent.Combination.Should().Be("Straight");
        }
    }
}