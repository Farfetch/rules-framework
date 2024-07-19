namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.Generic;

    public partial class Scenario8Data : IScenarioData<PokerRulesets, PokerConditions>
    {
        private IEnumerable<Rule<PokerRulesets, PokerConditions>> GetStraightFlushRules()
        {
            return new[]
            {
                // Straight flush of Clubs:
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Clubs: 6, 5, 4, 3, 2")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.DeuceOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.TreyOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.FourOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.FiveOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.SixOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Clubs: 7, 6, 5, 4, 3")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.TreyOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.FourOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.FiveOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.SixOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Clubs: 8, 7, 6, 5, 4")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.FourOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.FiveOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.SixOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.EightOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Clubs: 9, 8, 7, 6, 5")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.FiveOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.SixOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.EightOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.NineOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Clubs: 10, 9, 8, 7, 6")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.SixOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.EightOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.NineOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.TenOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Clubs: Jack, 10, 9, 8, 7")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.SevenOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.EightOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.NineOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.TenOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.JackOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Clubs: Queen, Jack, 10, 9, 8")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.EightOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.NineOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.TenOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.JackOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.QueenOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Clubs: King, Queen, Jack, 10, 9")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.NineOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.TenOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.JackOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.QueenOfClubs, Operators.Equal, true)
                            .Value(PokerConditions.KingOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,

                // Straight flush of Diamonds:
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Diamonds: 6, 5, 4, 3, 2")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.DeuceOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.TreyOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.FourOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.FiveOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.SixOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Diamonds: 7, 6, 5, 4, 3")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.TreyOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.FourOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.FiveOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.SixOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Diamonds: 8, 7, 6, 5, 4")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.FourOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.FiveOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.SixOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.EightOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Diamonds: 9, 8, 7, 6, 5")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.FiveOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.SixOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.EightOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.NineOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Diamonds: 10, 9, 8, 7, 6")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.SixOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.EightOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.NineOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.TenOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Diamonds: Jack, 10, 9, 8, 7")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.SevenOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.EightOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.NineOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.TenOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.JackOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Diamonds: Queen, Jack, 10, 9, 8")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.EightOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.NineOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.TenOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.JackOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.QueenOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Diamonds: King, Queen, Jack, 10, 9")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.NineOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.TenOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.JackOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.QueenOfDiamonds, Operators.Equal, true)
                            .Value(PokerConditions.KingOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,

                // Straight flush of Hearts:
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Hearts: 6, 5, 4, 3, 2")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.DeuceOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.TreyOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.FourOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.FiveOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.SixOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Hearts: 7, 6, 5, 4, 3")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.TreyOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.FourOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.FiveOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.SixOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Hearts: 8, 7, 6, 5, 4")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.FourOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.FiveOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.SixOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.EightOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Hearts: 9, 8, 7, 6, 5")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.FiveOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.SixOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.EightOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.NineOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Hearts: 10, 9, 8, 7, 6")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.SixOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.EightOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.NineOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.TenOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Hearts: Jack, 10, 9, 8, 7")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.SevenOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.EightOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.NineOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.TenOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.JackOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Hearts: Queen, Jack, 10, 9, 8")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.EightOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.NineOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.TenOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.JackOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.QueenOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Hearts: King, Queen, Jack, 10, 9")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.NineOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.TenOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.JackOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.QueenOfHearts, Operators.Equal, true)
                            .Value(PokerConditions.KingOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,

                // Straight flush of Spades:
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Spades: 6, 5, 4, 3, 2")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.DeuceOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.TreyOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.FourOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.FiveOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.SixOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Spades: 7, 6, 5, 4, 3")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.TreyOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.FourOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.FiveOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.SixOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Spades: 8, 7, 6, 5, 4")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.FourOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.FiveOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.SixOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.EightOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Spades: 9, 8, 7, 6, 5")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.FiveOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.SixOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.EightOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.NineOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Spades: 10, 9, 8, 7, 6")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.SixOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.SevenOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.EightOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.NineOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.TenOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Spades: Jack, 10, 9, 8, 7")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.SevenOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.EightOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.NineOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.TenOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.JackOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Spades: Queen, Jack, 10, 9, 8")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.EightOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.NineOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.TenOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.JackOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.QueenOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.Create<PokerRulesets, PokerConditions>("Benchmark 3 - Straight flush of Spades: King, Queen, Jack, 10, 9")
                    .OnRuleset(PokerRulesets.TexasHoldemPokerSingleCombinations)
                    .SetContent(new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .Since(DateTime.Parse("2000-01-01"))
                    .ApplyWhen(c => c
                        .And(x => x
                            .Value(PokerConditions.NineOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.TenOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.JackOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.QueenOfSpades, Operators.Equal, true)
                            .Value(PokerConditions.KingOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
            };
        }
    }
}