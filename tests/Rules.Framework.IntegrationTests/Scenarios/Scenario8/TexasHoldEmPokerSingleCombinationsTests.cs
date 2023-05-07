namespace Rules.Framework.IntegrationTests.Scenarios.Scenario8
{
    using System;
    using System.Threading.Tasks;
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
        public async Task PokerCombinations_Given_EvaluatesStraightCombination(bool enableCompilation)
        {
            // Arrange
            var matchDate = new DateTime(2023, 1, 1);
            var conditions = new[]
            {
                new Condition<ConditionTypes>(ConditionTypes.NumberOfKings, 1),
                new Condition<ConditionTypes>(ConditionTypes.NumberOfQueens, 1),
                new Condition<ConditionTypes>(ConditionTypes.NumberOfJacks, 1 ),
                new Condition<ConditionTypes>(ConditionTypes.NumberOfTens, 1 ),
                new Condition<ConditionTypes>(ConditionTypes.NumberOfNines, 1),
                new Condition<ConditionTypes>(ConditionTypes.KingOfClubs, true),
                new Condition<ConditionTypes>(ConditionTypes.QueenOfDiamonds, true),
                new Condition<ConditionTypes>(ConditionTypes.JackOfClubs, true),
                new Condition<ConditionTypes>(ConditionTypes.TenOfHearts, true),
                new Condition<ConditionTypes>(ConditionTypes.NineOfSpades, true),
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