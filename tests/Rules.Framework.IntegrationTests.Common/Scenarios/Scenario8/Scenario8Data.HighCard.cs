namespace Rules.Framework.IntegrationTests.Common.Scenarios.Scenario8
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;

    public partial class Scenario8Data : IScenarioData<ContentTypes, ConditionTypes>
    {
        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetHighCardsRules()
        {
            return new[]
            {
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - High Card Deuces")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(ConditionTypes.NumberOfDeuces, Operators.Equal, 1)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - High Card Treys")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(ConditionTypes.NumberOfTreys, Operators.Equal, 1)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - High Card Fours")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(ConditionTypes.NumberOfFours, Operators.Equal, 1)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - High Card Fives")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(ConditionTypes.NumberOfFives, Operators.Equal, 1)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - High Card Sixes")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(ConditionTypes.NumberOfSixes, Operators.Equal, 1)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - High Card Sevens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(ConditionTypes.NumberOfSevens, Operators.Equal, 1)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - High Card Eights")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(ConditionTypes.NumberOfEigths, Operators.Equal, 1)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - High Card Nines")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(ConditionTypes.NumberOfNines, Operators.Equal, 1)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - High Card Tens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(ConditionTypes.NumberOfTens, Operators.Equal, 1)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - High Card Jacks")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(ConditionTypes.NumberOfJacks, Operators.Equal, 1)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - High Card Queens")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(ConditionTypes.NumberOfQueens, Operators.Equal, 1)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - High Card Kings")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(ConditionTypes.NumberOfKings, Operators.Equal, 1)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - High Card Aces")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "High Card" })
                    .WithCondition(ConditionTypes.NumberOfAces, Operators.Equal, 1)
                    .Build().Rule,
            };
        }
    }
}