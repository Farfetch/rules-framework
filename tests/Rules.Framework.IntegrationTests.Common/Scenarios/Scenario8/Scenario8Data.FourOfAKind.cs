namespace Rules.Framework.IntegrationTests.Common.Scenarios.Scenario8
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;

    public partial class Scenario8Data : IScenarioData<ContentTypes, ConditionTypes>
    {
        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetFourOfAKindRules()
        {
            return new[]
            {
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Four Of A Kind Deuces")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(ConditionTypes.NumberOfDeuces, Operators.Equal, 4)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Four Of A Kind Treys")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(ConditionTypes.NumberOfTreys, Operators.Equal, 4)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Four Of A Kind Fours")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(ConditionTypes.NumberOfFours, Operators.Equal, 4)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Four Of A Kind Fives")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(ConditionTypes.NumberOfFives, Operators.Equal, 4)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Four Of A Kind Sixes")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(ConditionTypes.NumberOfSixes, Operators.Equal, 4)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Four Of A Kind Sevens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(ConditionTypes.NumberOfSevens, Operators.Equal, 4)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Four Of A Kind Eights")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(ConditionTypes.NumberOfEigths, Operators.Equal, 4)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Four Of A Kind Nines")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Three Of A Kind" })
                    .WithCondition(ConditionTypes.NumberOfNines, Operators.Equal, 4)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Four Of A Kind Tens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(ConditionTypes.NumberOfTens, Operators.Equal, 4)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Four Of A Kind Jacks")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(ConditionTypes.NumberOfJacks, Operators.Equal, 4)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Four Of A Kind Queens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(ConditionTypes.NumberOfQueens, Operators.Equal, 4)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Four Of A Kind Kings")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(ConditionTypes.NumberOfKings, Operators.Equal, 4)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Four Of A Kind Aces")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Four Of A Kind" })
                    .WithCondition(ConditionTypes.NumberOfAces, Operators.Equal, 4)
                    .Build().Rule,
            };
        }
    }
}