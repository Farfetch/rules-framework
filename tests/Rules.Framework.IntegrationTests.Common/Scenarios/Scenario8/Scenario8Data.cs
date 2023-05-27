namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework;
    using Rules.Framework.Core;

    public partial class Scenario8Data : IScenarioData<ContentTypes, ConditionTypes>
    {
        public IEnumerable<Condition<ConditionTypes>> Conditions => new[]
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

        public DateTime MatchDate => DateTime.Parse("2022-12-01");

        public IEnumerable<Rule<ContentTypes, ConditionTypes>> Rules => this.GetRules();

        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetRules()
        {
            // Does not consider the double pairs and full house combinations, as they would imply a
            // combinatorial explosion. For the purpose of the benchmark, scenario already simulates
            // a high number of rules.
            var highCards = this.GetHighCardsRules();

            var pairs = this.GetPairsRules();

            var threeOfAKind = this.GetThreeOfAKindRules();

            var straights = this.GetStraightRules();

            var flushs = this.GetFlushRules();

            var fourOfAKind = this.GetFourOfAKindRules();

            var straightFlushs = this.GetStraightFlushRules();

            var royalFlushs = this.GetRoyalFlushRules();

            return highCards.Concat(pairs).Concat(threeOfAKind).Concat(straights).Concat(flushs).Concat(fourOfAKind).Concat(straightFlushs).Concat(royalFlushs);
        }
    }
}