namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework;
    using Rules.Framework.Generic;

    public partial class Scenario8Data : IScenarioData<PokerRulesets, PokerConditions>
    {
        public IEnumerable<Condition<PokerConditions>> Conditions => new[]
        {
            new Condition<PokerConditions>(PokerConditions.NumberOfKings, 1),
            new Condition<PokerConditions>(PokerConditions.NumberOfQueens, 1 ),
            new Condition<PokerConditions>(PokerConditions.NumberOfJacks, 1),
            new Condition<PokerConditions>(PokerConditions.NumberOfTens, 1 ),
            new Condition<PokerConditions>(PokerConditions.NumberOfNines, 1 ),
            new Condition<PokerConditions>(PokerConditions.KingOfClubs, true ),
            new Condition<PokerConditions>(PokerConditions.QueenOfDiamonds, true ),
            new Condition<PokerConditions>(PokerConditions.JackOfClubs, true ),
            new Condition<PokerConditions>(PokerConditions.TenOfHearts, true ),
            new Condition<PokerConditions>(PokerConditions.NineOfSpades, true ),
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