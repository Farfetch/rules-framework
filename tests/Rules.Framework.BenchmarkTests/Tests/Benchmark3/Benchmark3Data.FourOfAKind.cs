namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal partial class Benchmark3Data
    {
        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetFourOfAKindRules()
        {
            return new[]
            {
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Four Of A Kind Deuces")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfDeuces)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(4)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Four Of A Kind Treys")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfTreys)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(4)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Four Of A Kind Fours")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfFours)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(4)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Four Of A Kind Fives")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfFives)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(4)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Four Of A Kind Sixes")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfSixes)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(4)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Four Of A Kind Sevens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfSevens)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(4)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Four Of A Kind Eights")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfEigths)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(4)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Four Of A Kind Nines")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfNines)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(4)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Four Of A Kind Tens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfTens)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(4)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Four Of A Kind Jacks")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfJacks)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(4)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Four Of A Kind Queens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfQueens)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(4)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Four Of A Kind Kings")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfKings)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(4)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Four Of A Kind Aces")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfAces)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(4)
                            .Build())
                    .Build().Rule,
            };
        }
    }
}
