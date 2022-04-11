namespace Rules.Framework.Tests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class ConditionTypeExtractorTests
    {
        [Fact]
        public void GetConditionTypes_ReturnsCorrectExtraction()
        {
            // Arrange

            var dateBegin = new DateTime(2018, 01, 01);
            var dateEnd = new DateTime(2019, 01, 01);

            var contentType = ContentType.Type1;

            var rule1 = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = dateBegin,
                DateEnd = dateEnd,
                Name = "Rule 1",
                Priority = 3,
                RootCondition = new StringConditionNode<ConditionType>(ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            var rule2 = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2020, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Rule 2",
                Priority = 200,
                RootCondition = new StringConditionNode<ConditionType>(ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            var rule3 = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = dateBegin,
                DateEnd = dateEnd,
                Name = "Rule 3",
                Priority = 1,
                RootCondition = new StringConditionNode<ConditionType>(ConditionType.IsoCurrency, Operators.Equal, "EUR")
            };

            var rule4 = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = dateBegin,
                DateEnd = dateEnd,
                Name = "Rule 4",
                Priority = 1,
                RootCondition = new ComposedConditionNode<ConditionType>(
                LogicalOperators.And,
                new IConditionNode<ConditionType>[]
                {
                    new StringConditionNode<ConditionType>(ConditionType.IsVip, Operators.Equal, "true"),
                    new StringConditionNode<ConditionType>(ConditionType.PluviosityRate, Operators.Equal, "15"),
                    new StringConditionNode<ConditionType>(ConditionType.IsoCurrency, Operators.Equal, "JPY")
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

            var expectedConditionTypeList = new List<ConditionType>()
            {
                ConditionType.IsoCurrency,
                ConditionType.IsoCountryCode,
                ConditionType.IsVip,
                ConditionType.PluviosityRate
            };

            var conditionTypeExtractor = new ConditionTypeExtractor<ContentType, ConditionType>();

            // Act
            var actual = conditionTypeExtractor.GetConditionTypes(matchRules);

            // Assert
            actual.Should().BeEquivalentTo(expectedConditionTypeList);
        }

        [Fact]
        public void GetConditionTypes_WithEmptyMatchRules_ReturnsEmptyListConditionTypes()
        {
            // Arrange

            var matchRules = new List<Rule<ContentType, ConditionType>>();

            var expectedConditionTypeList = new List<ConditionType>();

            var conditionTypeExtractor = new ConditionTypeExtractor<ContentType, ConditionType>();

            // Act
            var actual = conditionTypeExtractor.GetConditionTypes(matchRules);

            // Assert
            actual.Should().BeEquivalentTo(expectedConditionTypeList);
        }
    }
}