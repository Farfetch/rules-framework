namespace Rules.Framework.Tests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class ConditionTypeExtractorTests
    {
        [Fact]
        public void GetConditionTypes_ReturnsCorrectExtraction()
        {
            // Arrange

            var dateBegin = new DateTime(2018, 01, 01);
            var dateEnd = new DateTime(2019, 01, 01);

            var contentType = ContentType.Type1.ToString();

            var rule1 = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = dateBegin,
                DateEnd = dateEnd,
                Name = "Rule 1",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var rule2 = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2020, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Rule 2",
                Priority = 200,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var rule3 = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = dateBegin,
                DateEnd = dateEnd,
                Name = "Rule 3",
                Priority = 1,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCurrency.ToString(), Operators.Equal, "EUR")
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
                    new ValueConditionNode(DataTypes.String,ConditionType.IsVip.ToString(), Operators.Equal, "true"),
                    new ValueConditionNode(DataTypes.String,ConditionType.PluviosityRate.ToString(), Operators.Equal, "15"),
                    new ValueConditionNode(DataTypes.String,ConditionType.IsoCurrency.ToString(), Operators.Equal, "JPY")
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
                ConditionType.IsoCurrency.ToString(),
                ConditionType.IsoCountryCode.ToString(),
                ConditionType.IsVip.ToString(),
                ConditionType.PluviosityRate.ToString(),
            };

            var conditionTypeExtractor = new ConditionTypeExtractor();

            // Act
            var actual = conditionTypeExtractor.GetConditionTypes(matchRules);

            // Assert
            actual.Should().BeEquivalentTo(expectedConditionTypeList);
        }

        [Fact]
        public void GetConditionTypes_WithEmptyMatchRules_ReturnsEmptyListConditionTypes()
        {
            // Arrange

            var matchRules = new List<Rule>();

            var expectedConditionTypeList = new List<string>();

            var conditionTypeExtractor = new ConditionTypeExtractor();

            // Act
            var actual = conditionTypeExtractor.GetConditionTypes(matchRules);

            // Assert
            actual.Should().BeEquivalentTo(expectedConditionTypeList);
        }

        [Fact]
        public void GetConditionTypes_WithNullRootCondition_ReturnsEmptyListConditionTypes()
        {
            // Arrange

            var dateBegin = new DateTime(2018, 01, 01);
            var dateEnd = new DateTime(2019, 01, 01);

            var contentType = ContentType.Type1.ToString();

            var matchRules = new List<Rule>
            {
                new Rule
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

            var conditionTypeExtractor = new ConditionTypeExtractor();

            // Act
            var actual = conditionTypeExtractor.GetConditionTypes(matchRules);

            // Assert
            actual.Should().BeEquivalentTo(expectedConditionTypeList);
        }
    }
}