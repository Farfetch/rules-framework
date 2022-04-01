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

            DateTime dateBegin = new DateTime(2018, 01, 01);
            DateTime dateEnd = new DateTime(2019, 01, 01);

            ContentType contentType = ContentType.Type1;

            Rule<ContentType, ConditionType> rule1 = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = dateBegin,
                DateEnd = dateEnd,
                Name = "Rule 1",
                Priority = 3,
                RootCondition = new StringConditionNode<ConditionType>(ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            Rule<ContentType, ConditionType> rule2 = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2020, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Rule 2",
                Priority = 200,
                RootCondition = new StringConditionNode<ConditionType>(ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            Rule<ContentType, ConditionType> rule3 = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = dateBegin,
                DateEnd = dateEnd,
                Name = "Rule 3",
                Priority = 1,
                RootCondition = new StringConditionNode<ConditionType>(ConditionType.IsoCurrency, Operators.Equal, "EUR")
            };

            var matchRules = new[]
            {
                rule1,
                rule2,
                rule3
            };

            var expectedConditionTypeList = new List<ConditionType>()
            {
                ConditionType.IsoCurrency,
                ConditionType.IsoCountryCode
            };

            var conditionTypeExtractor = new ConditionTypeExtractor<ContentType, ConditionType>();

            // Act
            var actual = conditionTypeExtractor.GetConditionTypes(matchRules);

            // Assert
            actual.Should().BeEquivalentTo(expectedConditionTypeList);
        }
    }
}