namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;

    internal partial class Benchmark3Data : IBenchmarkData<ContentTypes, ConditionTypes>
    {
        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetRoyalFlushRules()
        {
            return new[]
            {
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Royal flush of Clubs: Ace, King, Queen, Jack, 10")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Royal Flush" })
                    .WithCondition(x =>
                        x.AsComposed()
                            .WithLogicalOperator(LogicalOperators.And)
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.TenOfClubs)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.JackOfClubs)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.QueenOfClubs)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.KingOfClubs)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.AceOfClubs)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .Build()
                        )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Royal flush of Diamonds: Ace, King, Queen, Jack, 10")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Royal Flush" })
                    .WithCondition(x =>
                        x.AsComposed()
                            .WithLogicalOperator(LogicalOperators.And)
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.TenOfDiamonds)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.JackOfDiamonds)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.QueenOfDiamonds)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.KingOfDiamonds)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.AceOfDiamonds)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .Build()
                        )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Royal flush of Hearts: Ace, King, Queen, Jack, 10")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Royal Flush" })
                    .WithCondition(x =>
                        x.AsComposed()
                            .WithLogicalOperator(LogicalOperators.And)
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.TenOfHearts)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.JackOfHearts)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.QueenOfHearts)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.KingOfHearts)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.AceOfHearts)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .Build()
                        )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Royal flush of Spades: Ace, King, Queen, Jack, 10")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Royal Flush" })
                    .WithCondition(x =>
                        x.AsComposed()
                            .WithLogicalOperator(LogicalOperators.And)
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.TenOfSpades)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.JackOfSpades)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.QueenOfSpades)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.KingOfSpades)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .AddCondition(c =>
                                c.AsValued(ConditionTypes.AceOfSpades)
                                    .OfDataType<bool>()
                                    .WithComparisonOperator(Operators.Equal)
                                    .SetOperand(true)
                                    .Build()
                            )
                            .Build()
                        )
                    .Build().Rule,
            };
        }
    }
}