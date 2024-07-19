namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.Generic;

    public partial class Scenario8Data : IScenarioData<ContentTypes, ConditionTypes>
    {
        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetStraightFlushRules()
        {
            return new[]
            {
                // Straight flush of Clubs:
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Clubs: 6, 5, 4, 3, 2")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.DeuceOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.TreyOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.FourOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.FiveOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Clubs: 7, 6, 5, 4, 3")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.TreyOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.FourOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.FiveOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Clubs: 8, 7, 6, 5, 4")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.FourOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.FiveOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Clubs: 9, 8, 7, 6, 5")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.FiveOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Clubs: 10, 9, 8, 7, 6")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.SixOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Clubs: Jack, 10, 9, 8, 7")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.SevenOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Clubs: Queen, Jack, 10, 9, 8")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.EightOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.QueenOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Clubs: King, Queen, Jack, 10, 9")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.NineOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.QueenOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.KingOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,

                // Straight flush of Diamonds:
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Diamonds: 6, 5, 4, 3, 2")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.DeuceOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.TreyOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.FourOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.FiveOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Diamonds: 7, 6, 5, 4, 3")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.TreyOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.FourOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.FiveOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Diamonds: 8, 7, 6, 5, 4")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.FourOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.FiveOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Diamonds: 9, 8, 7, 6, 5")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.FiveOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Diamonds: 10, 9, 8, 7, 6")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.SixOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Diamonds: Jack, 10, 9, 8, 7")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.SevenOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Diamonds: Queen, Jack, 10, 9, 8")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.EightOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.QueenOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Diamonds: King, Queen, Jack, 10, 9")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.NineOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.QueenOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.KingOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,

                // Straight flush of Hearts:
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Hearts: 6, 5, 4, 3, 2")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.DeuceOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.TreyOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.FourOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.FiveOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Hearts: 7, 6, 5, 4, 3")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.TreyOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.FourOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.FiveOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Hearts: 8, 7, 6, 5, 4")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.FourOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.FiveOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Hearts: 9, 8, 7, 6, 5")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.FiveOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Hearts: 10, 9, 8, 7, 6")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.SixOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Hearts: Jack, 10, 9, 8, 7")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.SevenOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Hearts: Queen, Jack, 10, 9, 8")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.EightOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.QueenOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Hearts: King, Queen, Jack, 10, 9")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.NineOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.QueenOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.KingOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,

                // Straight flush of Spades:
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Spades: 6, 5, 4, 3, 2")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.DeuceOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.TreyOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.FourOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.FiveOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Spades: 7, 6, 5, 4, 3")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.TreyOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.FourOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.FiveOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Spades: 8, 7, 6, 5, 4")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.FourOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.FiveOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Spades: 9, 8, 7, 6, 5")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.FiveOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.SixOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Spades: 10, 9, 8, 7, 6")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.SixOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.SevenOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Spades: Jack, 10, 9, 8, 7")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.SevenOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.EightOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Spades: Queen, Jack, 10, 9, 8")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.EightOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.NineOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.QueenOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                Rule.New<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight flush of Spades: King, Queen, Jack, 10, 9")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.NineOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.TenOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.QueenOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.KingOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
            };
        }
    }
}