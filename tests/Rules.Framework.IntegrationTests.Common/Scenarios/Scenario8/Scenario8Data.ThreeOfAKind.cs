namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
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
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfDeuces, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Treys")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfTreys, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Fours")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfDeuces, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Fives")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfFives, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Sixes")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfSixes, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Sevens")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfSevens, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Eights")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfEigths, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Nines")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfNines, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Tens")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfTens, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Jacks")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfJacks, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Queens")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfQueens, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Kings")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfKings, Operators.Equal, 3)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Three Of A Kind Aces")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfAces, Operators.Equal, 3)
                    .Build().Rule,
            };
        }
    }
}