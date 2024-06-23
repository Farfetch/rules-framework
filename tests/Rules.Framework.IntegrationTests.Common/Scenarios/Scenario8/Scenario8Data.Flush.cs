namespace Rules.Framework.IntegrationTests.Common.Scenarios.Scenario8
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;

    public partial class Scenario8Data : IScenarioData<ContentTypes, ConditionTypes>
    {
        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetFlushRules()
        {
            return new[]
            {
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Flush of Clubs")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Flush" })
                    .WithCondition(ConditionTypes.NumberOfClubs, Operators.GreaterThanOrEqual, 5)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Flush of Diamonds")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Flush" })
                    .WithCondition(ConditionTypes.NumberOfDiamonds, Operators.GreaterThanOrEqual, 5)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Flush of Hearts")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Flush" })
                    .WithCondition(ConditionTypes.NumberOfHearts, Operators.GreaterThanOrEqual, 5)
                    .Build().Rule,
                RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                    .WithName("Scenario 8 - Flush of Spades")
                    .WithDateBegin(DateTime.Parse("2000-01-01"))
                    .WithContent(ContentTypes.TexasHoldemPokerSingleCombinations, new SingleCombinationPokerScore { Combination = "Flush" })
                    .WithCondition(ConditionTypes.NumberOfSpades, Operators.GreaterThanOrEqual, 5)
                    .Build().Rule,
            };
        }
    }
}