namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Generic;

    public partial class Scenario8Data : IScenarioData<PokerRulesets, PokerConditions>
    {
        public IDictionary<PokerConditions, object> Conditions => new Dictionary<PokerConditions, object>
        {
            { PokerConditions.NumberOfKings, 1 },
            { PokerConditions.NumberOfQueens, 1 },
            { PokerConditions.NumberOfJacks, 1 },
            { PokerConditions.NumberOfTens, 1 },
            { PokerConditions.NumberOfNines, 1 },
            { PokerConditions.KingOfClubs, true },
            { PokerConditions.QueenOfDiamonds, true },
            { PokerConditions.JackOfClubs, true },
            { PokerConditions.TenOfHearts, true },
            { PokerConditions.NineOfSpades, true },
        };

        public DateTime MatchDate => DateTime.Parse("2022-12-01");

        public IEnumerable<Rule<PokerRulesets, PokerConditions>> Rules => this.GetRules();

        private IEnumerable<Rule<PokerRulesets, PokerConditions>> GetRules()
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