namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.Generic;

    public partial class Scenario8Data : IScenarioData<PokerRulesets, PokerConditions>
    {
        private IEnumerable<Rule<PokerRulesets, PokerConditions>> GetRoyalFlushRules()
        {
            return new[]
            {
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Royal flush of Clubs: Ace, King, Queen, Jack, 10")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Royal Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.TenOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.JackOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.QueenOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.KingOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.AceOfClubs, Operators.Equal, true)
                            )
                        )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Royal flush of Diamonds: Ace, King, Queen, Jack, 10")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Royal Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.TenOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.JackOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.QueenOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.KingOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.AceOfDiamonds, Operators.Equal, true)
                            )
                        )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Royal flush of Hearts: Ace, King, Queen, Jack, 10")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Royal Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.TenOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.JackOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.QueenOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.KingOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.AceOfHearts, Operators.Equal, true)
                            )
                        )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Royal flush of Spades: Ace, King, Queen, Jack, 10")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Royal Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.TenOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.JackOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.QueenOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.KingOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.AceOfSpades, Operators.Equal, true)
                            )
                        )
                    .Build().Rule,
            };
        }
    }
}