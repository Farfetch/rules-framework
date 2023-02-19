namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;

    internal partial class Benchmark3Data
    {
        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetHighCardsRules()
        {
            return new[]
            {
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - High Card Deuces")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfDeuces)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(1)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - High Card Treys")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfTreys)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(1)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - High Card Fours")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfFours)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(1)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - High Card Fives")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfFives)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(1)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - High Card Sixes")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfSixes)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(1)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - High Card Sevens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfSevens)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(1)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - High Card Eights")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfEigths)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(1)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - High Card Nines")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfNines)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(1)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - High Card Tens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfTens)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(1)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - High Card Jacks")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfJacks)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(1)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - High Card Queens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfQueens)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(1)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - High Card Kings")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfKings)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(1)
                            .Build())
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - High Card Aces")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfAces)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.Equal)
                            .SetOperand(1)
                            .Build())
                    .Build().Rule,
            };
        }
    }
}