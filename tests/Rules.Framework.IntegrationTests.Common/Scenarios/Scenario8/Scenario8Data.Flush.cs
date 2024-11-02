namespace Rules.Framework.IntegrationTests.Common.Scenarios.Scenario8
{
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.Generic;

    public partial class Scenario8Data : IScenarioData<PokerRulesets, PokerConditions>
    {
        private IEnumerable<Rule<PokerRulesets, PokerConditions>> GetFlushRules()
        {
            return new[]
            {
                Rule.Create<PokerRulesets, PokerConditions>("Scenario 8 - Flush of Clubs")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Flush" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfClubs, Operators.GreaterThanOrEqual, 5)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Scenario 8 - Flush of Diamonds")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Flush" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfDiamonds, Operators.GreaterThanOrEqual, 5)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Scenario 8 - Flush of Hearts")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Flush" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfHearts, Operators.GreaterThanOrEqual, 5)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Scenario 8 - Flush of Spades")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Flush" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfSpades, Operators.GreaterThanOrEqual, 5)
                    .Build().Rule,
            };
        }
    }
}