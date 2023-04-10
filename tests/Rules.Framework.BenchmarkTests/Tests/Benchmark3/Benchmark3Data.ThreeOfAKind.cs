namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;

    internal partial class Benchmark3Data : IBenchmarkData<ContentTypes, ConditionTypes>
    {
        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetThreeOfAKindRules()
        {
            return new[]
            {
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Three Of A Kind Deuces")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfDeuces)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Three Of A Kind Treys")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfTreys)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Three Of A Kind Fours")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfFours)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Three Of A Kind Fives")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfFives)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Three Of A Kind Sixes")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfSixes)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Three Of A Kind Sevens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfSevens)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Three Of A Kind Eights")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfEigths)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Three Of A Kind Nines")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfNines)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Three Of A Kind Tens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfTens)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Three Of A Kind Jacks")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfJacks)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Three Of A Kind Queens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfQueens)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Three Of A Kind Kings")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfKings)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Three Of A Kind Aces")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfAces)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(3)
                            .Build())
                    .Build().Rule,
            };
        }
    }
}