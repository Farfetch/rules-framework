namespace Rules.Framework.Tests.Extensions
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Rules.Framework.Extensions;
    using Rules.Framework.Generics;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class GenericSearchArgsExtensionsTests
    {
        [Fact]
        public void GenericSearchArgsExtensions_ToSearchArgs_WithConditions_Success()
        {
            //Arrange
            var contentType = ContentType.Type1;
            var dateBegin = new DateTime(2018, 01, 01);
            var dateEnd = new DateTime(2020, 12, 31);
            var pluviosityRate = 132000.00m;
            var countryCode = "USA";

            var expectedSearchArgs = new SearchArgs<ContentType, ConditionType>(contentType, dateBegin, dateEnd)
            {
                Conditions = new List<Condition<ConditionType>>
                {
                    new Condition<ConditionType>
                    {
                        Type = ConditionType.PluviosityRate,
                        Value = pluviosityRate
                    },
                    new Condition<ConditionType>
                    {
                        Type = ConditionType.IsoCountryCode,
                        Value = countryCode
                    }
                },
                ExcludeRulesWithoutSearchConditions = true
            };

            var contentTypeCode = "Type1";

            var genericSearchArgs = new SearchArgs<GenericContentType, GenericConditionType>(
                new GenericContentType { Identifier = contentTypeCode }, dateBegin, dateEnd
                )
            {
                Conditions = new List<Condition<GenericConditionType>>
                {
                    new Condition<GenericConditionType>
                    {
                        Type = new GenericConditionType { Identifier = "PluviosityRate" },
                        Value = pluviosityRate
                    },
                    new Condition<GenericConditionType>
                    {
                        Type = new GenericConditionType { Identifier = "IsoCountryCode" },
                        Value = countryCode
                    }
                },
                ExcludeRulesWithoutSearchConditions = true
            };

            // Act
            var convertedSearchArgs = genericSearchArgs.ToSearchArgs<ContentType, ConditionType>();

            // Assert
            convertedSearchArgs.Should().BeEquivalentTo(expectedSearchArgs);
        }

        [Fact]
        public void GenericSearchArgsExtensions_ToSearchArgs_WithInvalidType_ThrowsException()
        {
            //Arrange
            var contentTypeCode = "Type1";
            var dateBegin = new DateTime(2018, 01, 01);
            var dateEnd = new DateTime(2020, 12, 31);

            var genericSearchArgs = new SearchArgs<GenericContentType, GenericConditionType>(
                new GenericContentType { Identifier = contentTypeCode }, dateBegin, dateEnd
                );

            // Act and Assert
            Assert.Throws<ArgumentException>(() => genericSearchArgs.ToSearchArgs<ContentTypeClass, ConditionType>());
        }

        [Fact]
        public void GenericSearchArgsExtensions_ToSearchArgs_WithoutConditions_Success()
        {
            //Arrange
            var contentTypeCode = "Type1";
            var contentType = ContentType.Type1;
            var dateBegin = new DateTime(2018, 01, 01);
            var dateEnd = new DateTime(2020, 12, 31);

            var expectedSearchArgs = new SearchArgs<ContentType, ConditionType>(contentType, dateBegin, dateEnd);

            var genericSearchArgs = new SearchArgs<GenericContentType, GenericConditionType>(
                new GenericContentType { Identifier = contentTypeCode }, dateBegin, dateEnd
                );

            // Act
            var convertedSearchArgs = genericSearchArgs.ToSearchArgs<ContentType, ConditionType>();

            // Assert
            convertedSearchArgs.Should().BeEquivalentTo(expectedSearchArgs);
        }
    }
}