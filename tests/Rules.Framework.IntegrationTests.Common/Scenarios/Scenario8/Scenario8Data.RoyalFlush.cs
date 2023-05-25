namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;

    public partial class Scenario8Data : IScenarioData<ContentTypes, ConditionTypes>
    {
        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetRoyalFlushRules()
        {
            return new[]
            {
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Royal flush of Clubs: Ace, King, Queen, Jack, 10")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Royal Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.TenOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.QueenOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.KingOfClubs, Operators.Equal, true)
                            .Value(ConditionTypes.AceOfClubs, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Royal flush of Diamonds: Ace, King, Queen, Jack, 10")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Royal Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.TenOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.QueenOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.KingOfDiamonds, Operators.Equal, true)
                            .Value(ConditionTypes.AceOfDiamonds, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Royal flush of Hearts: Ace, King, Queen, Jack, 10")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Royal Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.TenOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.QueenOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.KingOfHearts, Operators.Equal, true)
                            .Value(ConditionTypes.AceOfHearts, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Royal flush of Spades: Ace, King, Queen, Jack, 10")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Royal Flush" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.TenOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.JackOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.QueenOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.KingOfSpades, Operators.Equal, true)
                            .Value(ConditionTypes.AceOfSpades, Operators.Equal, true)
                        )
                    )
                    .Build().Rule,
            };
        }
    }
}