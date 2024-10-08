namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.Generic;

    public partial class Scenario8Data : IScenarioData<PokerRulesets, PokerConditions>
    {
        private IEnumerable<Rule<PokerRulesets, PokerConditions>> GetStraightRules()
        {
            return new[]
            {
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight 6, 5, 4, 3, 2")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight" })
                    .Since("2000-01-01")
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.NumberOfDeuces, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfTreys, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfFours, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfFives, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfSixes, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight 7, 6, 5, 4, 3")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight" })
                    .Since("2000-01-01")
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.NumberOfTreys, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfFours, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfFives, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfSixes, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfSevens, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight 8, 7, 6, 5, 4")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight" })
                    .Since("2000-01-01")
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.NumberOfFours, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfFives, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfSixes, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfSevens, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfEigths, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight 9, 8, 7, 6, 5")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight" })
                    .Since("2000-01-01")
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.NumberOfFives, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfSixes, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfSevens, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfEigths, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfNines, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight 10, 9, 8, 7, 6")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight" })
                    .Since("2000-01-01")
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.NumberOfSixes, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfSevens, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfEigths, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfNines, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfTens, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight Jack, 10, 9, 8, 7")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight" })
                    .Since("2000-01-01")
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.NumberOfSevens, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfEigths, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfNines, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfTens, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfJacks, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight Queen, Jack, 10, 9, 8")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight" })
                    .Since("2000-01-01")
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.NumberOfEigths, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfNines, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfTens, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfJacks, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfQueens, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight King, Queen, Jack, 10, 9")
                    .InRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight" })
                    .Since("2000-01-01")
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.NumberOfNines, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfTens, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfJacks, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfQueens, Operators.GreaterThanOrEqual, 1)
                            .Value(PokerConditions.NumberOfKings, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
            };
        }
    }
}