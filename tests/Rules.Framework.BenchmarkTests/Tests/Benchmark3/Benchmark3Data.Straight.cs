namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;

    internal partial class Benchmark3Data : IBenchmarkData<ContentTypes, ConditionTypes>
    {
        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetStraightRules()
        {
            return new[]
            {
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight 6, 5, 4, 3, 2")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.NumberOfDeuces, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfTreys, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfFours, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfFives, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfSixes, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight 7, 6, 5, 4, 3")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.NumberOfTreys, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfFours, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfFives, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfSixes, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfSevens, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight 8, 7, 6, 5, 4")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.NumberOfFours, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfFives, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfSixes, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfSevens, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfEigths, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight 9, 8, 7, 6, 5")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.NumberOfFives, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfSixes, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfSevens, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfEigths, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfNines, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight 10, 9, 8, 7, 6")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.NumberOfSixes, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfSevens, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfEigths, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfNines, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfTens, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight Jack, 10, 9, 8, 7")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.NumberOfSevens, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfEigths, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfNines, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfTens, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfJacks, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight Queen, Jack, 10, 9, 8")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.NumberOfEigths, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfNines, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfTens, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfJacks, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfQueens, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Straight King, Queen, Jack, 10, 9")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Straight" })
                    .WithCondition(c => c
                        .And(x => x
                            .Value(ConditionTypes.NumberOfNines, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfTens, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfJacks, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfQueens, Operators.GreaterThanOrEqual, 1)
                            .Value(ConditionTypes.NumberOfKings, Operators.GreaterThanOrEqual, 1)
                        )
                    )
                    .Build().Rule,
            };
        }
    }
}