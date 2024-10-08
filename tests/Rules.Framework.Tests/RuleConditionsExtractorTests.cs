namespace Rules.Framework.Tests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RuleConditionsExtractorTests
    {
        [Fact]
        public void GetConditionTypes_ReturnsCorrectExtraction()
        {
            // Arrange

            var dateBegin = new DateTime(2018, 01, 01);
            var dateEnd = new DateTime(2019, 01, 01);

            var rule1 = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = dateBegin,
                DateEnd = dateEnd,
                Name = "Rule 1",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var rule2 = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2020, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Rule 2",
                Priority = 200,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var rule3 = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = dateBegin,
                DateEnd = dateEnd,
                Name = "Rule 3",
                Priority = 1,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCurrency.ToString(), Operators.Equal, "EUR")
            };

            var rule4 = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = dateBegin,
                DateEnd = dateEnd,
                Name = "Rule 4",
                Priority = 1,
                RootCondition = new ComposedConditionNode(
                LogicalOperators.And,
                new IConditionNode[]
                {
                    new ValueConditionNode(DataTypes.String,ConditionNames.IsVip.ToString(), Operators.Equal, "true"),
                    new ValueConditionNode(DataTypes.String,ConditionNames.PluviosityRate.ToString(), Operators.Equal, "15"),
                    new ValueConditionNode(DataTypes.String,ConditionNames.IsoCurrency.ToString(), Operators.Equal, "JPY")
                }
                )
            };

            var matchRules = new[]
            {
                rule1,
                rule2,
                rule3,
                rule4
            };

            var expectedConditionTypeList = new List<string>
            {
                ConditionNames.IsoCurrency.ToString(),
                ConditionNames.IsoCountryCode.ToString(),
                ConditionNames.IsVip.ToString(),
                ConditionNames.PluviosityRate.ToString(),
            };

            var conditionTypeExtractor = new RuleConditionsExtractor();

            // Act
            var actual = conditionTypeExtractor.GetConditions(matchRules);

            // Assert
            actual.Should().BeEquivalentTo(expectedConditionTypeList);
        }

        [Fact]
        public void GetConditionTypes_WithEmptyMatchRules_ReturnsEmptyListConditionTypes()
        {
            // Arrange

            var matchRules = new List<Rule>();

            var expectedConditionTypeList = new List<string>();

            var conditionTypeExtractor = new RuleConditionsExtractor();

            // Act
            var actual = conditionTypeExtractor.GetConditions(matchRules);

            // Assert
            actual.Should().BeEquivalentTo(expectedConditionTypeList);
        }

        [Fact]
        public void GetConditionTypes_WithNullRootCondition_ReturnsEmptyListConditionTypes()
        {
            // Arrange

            var dateBegin = new DateTime(2018, 01, 01);
            var dateEnd = new DateTime(2019, 01, 01);

            var matchRules = new List<Rule>
            {
                new()
                {
                    ContentContainer = new ContentContainer(_ => new object()),
                    DateBegin = dateBegin,
                    DateEnd = dateEnd,
                    Name = "Rule 3",
                    Priority = 1,
                    RootCondition = null
                }
            };

            var expectedConditionTypeList = new List<string>();

            var conditionTypeExtractor = new RuleConditionsExtractor();

            // Act
            var actual = conditionTypeExtractor.GetConditions(matchRules);

            // Assert
            actual.Should().BeEquivalentTo(expectedConditionTypeList);
        }
    }
}