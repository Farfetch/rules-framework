namespace Rules.Framework.IntegrationTests.Common.Scenarios.Scenario8
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
            new Condition<ConditionTypes>(ConditionTypes.NumberOfKings, 1),
            new Condition<ConditionTypes>(ConditionTypes.NumberOfQueens, 1 ),
            new Condition<ConditionTypes>(ConditionTypes.NumberOfJacks, 1),
            new Condition<ConditionTypes>(ConditionTypes.NumberOfTens, 1 ),
            new Condition<ConditionTypes>(ConditionTypes.NumberOfNines, 1 ),
            new Condition<ConditionTypes>(ConditionTypes.KingOfClubs, true ),
            new Condition<ConditionTypes>(ConditionTypes.QueenOfDiamonds, true ),
            new Condition<ConditionTypes>(ConditionTypes.JackOfClubs, true ),
            new Condition<ConditionTypes>(ConditionTypes.TenOfHearts, true ),
            new Condition<ConditionTypes>(ConditionTypes.NineOfSpades, true ),
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