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
        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetPairsRules()
        {
            return new[]
            {
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Pair Deuces")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Pair" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfDeuces)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(2)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Pair Treys")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Pair" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfTreys)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(2)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Pair Fours")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Pair" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfFours)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(2)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Pair Fives")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Pair" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfFives)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(2)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Pair Sixes")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Pair" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfSixes)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(2)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Pair Sevens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Pair" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfSevens)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(2)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Pair Eights")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Pair" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfEigths)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(2)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Pair Nines")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Pair" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfNines)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(2)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Pair Tens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Pair" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfTens)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(2)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Pair Jacks")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Pair" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfJacks)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(2)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Pair Queens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Pair" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfQueens)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(2)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Pair Kings")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Pair" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfKings)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(2)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Pair Aces")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Pair" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfAces)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(2)
                            .Build())
                    .Build().Rule,
            };
        }
    }
}
