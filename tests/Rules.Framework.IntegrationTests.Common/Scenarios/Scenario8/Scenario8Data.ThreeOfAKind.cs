namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.Generic;

    public partial class Scenario8Data : IScenarioData<PokerRulesets, PokerConditions>
    {
        private IEnumerable<Rule<PokerRulesets, PokerConditions>> GetThreeOfAKindRules()
        {
            return new[]
            {
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Deuces")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfDeuces, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Treys")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfTreys, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Fours")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfDeuces, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Fives")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfFives, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Sixes")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfSixes, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Sevens")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfSevens, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Eights")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfEigths, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Nines")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfNines, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Tens")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfTens, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Jacks")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfJacks, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Queens")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfQueens, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Kings")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfKings, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Aces")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since("2000-01-01")
                    .ApplyWhen(PokerConditions.NumberOfAces, Operators.Equal, 3)
                    .Build().Rule,
            };
        }
    }
}