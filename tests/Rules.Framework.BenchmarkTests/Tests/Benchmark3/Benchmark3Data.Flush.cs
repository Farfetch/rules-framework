namespace Rules.Framework.BenchmarkTests.Tests.Benchmark3
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;

    internal partial class Benchmark3Data
    {
        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetFlushRules()
        {
            return new[]
            {
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Flush of Clubs")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Flush" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfClubs)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.GreaterThanOrEqual)
                            .SetOperand(5)
                            .Build()
                        )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Flush of Diamonds")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Flush" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfDiamonds)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.GreaterThanOrEqual)
                            .SetOperand(5)
                            .Build()
                        )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Flush of Hearts")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Flush" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfHearts)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.GreaterThanOrEqual)
                            .SetOperand(5)
                            .Build()
                        )
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Benchmark 3 - Flush of Spades")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Flush" })
                    .WithCondition(x =>
                        x.AsValued(ConditionTypes.NumberOfSpades)
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.GreaterThanOrEqual)
                            .SetOperand(5)
                            .Build()
                        )
                    .Build().Rule,
            };
        }
    }
}