namespace Rules.Framework.IntegrationTests.Scenarios.Scenario8
{
    using System;
    using System.Collections.Generic;
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
            var conditions = new Dictionary<PokerConditions, object>
            {
                { PokerConditions.NumberOfKings, 1 },
                { PokerConditions.NumberOfQueens, 1 },
                { PokerConditions.NumberOfJacks, 1  },
                { PokerConditions.NumberOfTens, 1  },
                { PokerConditions.NumberOfNines, 1 },
                { PokerConditions.KingOfClubs, true },
                { PokerConditions.QueenOfDiamonds, true },
                { PokerConditions.JackOfClubs, true },
                { PokerConditions.TenOfHearts, true },
                { PokerConditions.NineOfSpades, true },
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