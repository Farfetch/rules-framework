namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.Generic;

    public partial class Scenario8Data : IScenarioData<PokerRulesets, PokerConditions>
    {
        private IEnumerable<Rule<PokerRulesets, PokerConditions>> GetHighCardsRules()
        {
            return new[]
            {
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - High Card Deuces")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "High Card" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfDeuces, Operators.Equal, 1)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - High Card Treys")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "High Card" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfTreys, Operators.Equal, 1)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - High Card Fours")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "High Card" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfFours, Operators.Equal, 1)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - High Card Fives")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "High Card" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfFives, Operators.Equal, 1)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - High Card Sixes")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "High Card" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfSixes, Operators.Equal, 1)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - High Card Sevens")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "High Card" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfSevens, Operators.Equal, 1)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - High Card Eights")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "High Card" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfEigths, Operators.Equal, 1)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - High Card Nines")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "High Card" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfNines, Operators.Equal, 1)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - High Card Tens")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "High Card" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfTens, Operators.Equal, 1)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - High Card Jacks")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "High Card" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfJacks, Operators.Equal, 1)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - High Card Queens")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "High Card" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfQueens, Operators.Equal, 1)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - High Card Kings")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "High Card" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfKings, Operators.Equal, 1)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - High Card Aces")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "High Card" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfAces, Operators.Equal, 1)
                    .Build().Rule,
            };
        }
    }
}