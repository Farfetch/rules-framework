namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.Generic;

    public partial class Scenario8Data : IScenarioData<PokerRulesets, PokerConditions>
    {
        private IEnumerable<Rule<PokerRulesets, PokerConditions>> GetPairsRules()
        {
            return new[]
            {
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Pair Deuces")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Pair" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfDeuces, Operators.Equal, 2)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Pair Treys")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Pair" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfTreys, Operators.Equal, 2)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Pair Fours")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Pair" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfFours, Operators.Equal, 2)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Pair Fives")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Pair" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfFives, Operators.Equal, 2)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Pair Sixes")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Pair" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfSixes, Operators.Equal, 2)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Pair Sevens")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Pair" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfSevens, Operators.Equal, 2)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Pair Eights")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Pair" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfEigths, Operators.Equal, 2)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Pair Nines")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Pair" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfNines, Operators.Equal, 2)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Pair Tens")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Pair" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfTens, Operators.Equal, 2)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Pair Jacks")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Pair" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfJacks, Operators.Equal, 2)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Pair Queens")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Pair" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfQueens, Operators.Equal, 2)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Pair Kings")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Pair" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfKings, Operators.Equal, 2)
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Pair Aces")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Pair" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(PokerConditions.NumberOfAces, Operators.Equal, 2)
                    .Build().Rule,
            };
        }
    }
}