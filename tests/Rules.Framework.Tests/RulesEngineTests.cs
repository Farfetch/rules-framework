using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rules.Framework.Core;
using Rules.Framework.Core.ConditionNodes;
using Rules.Framework.Evaluation;
using Rules.Framework.Tests.TestStubs;

namespace Rules.Framework.Tests
{
    [TestClass]
    public class RulesEngineTests
    {
        [TestMethod]
        public async Task RulesEngine_MatchOneAsync_GivenContentTypeDateAndConditions_FetchesRulesForDayEvalsAndReturnsTheTopmostPriorityOne()
        {
            // Arrange
            DateTime matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            ContentType contentType = ContentType.Type1;
            IEnumerable<Condition<ConditionType>> conditions = new[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsoCountryCode,
                    Value = "USA"
                },
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsoCurrency,
                    Value = "USD"
                }
            };

            Rule<ContentType, ConditionType> expected = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Expected rule",
                Priority = 3,
                RootCondition = new StringConditionNode<ConditionType>(ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            Rule<ContentType, ConditionType> other = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2010, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Expected rule",
                Priority = 200,
                RootCondition = new StringConditionNode<ConditionType>(ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            IEnumerable<Rule<ContentType, ConditionType>> rules = new[]
            {
                expected
            };
            Mock<IRulesDataSource<ContentType, ConditionType>> mockRulesDataSource = SetupMockForRulesDataSource(rules);
            Mock<IConditionsEvalEngine<ConditionType>> mockConditionsEvalEngine = SetupMockForConditionsEvalEngine(true);
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.Default;

            RulesEngine<ContentType, ConditionType> sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesDataSource.Object, rulesEngineOptions);

            // Act
            Rule<ContentType, ConditionType> actual = await sut.MatchOneAsync(contentType, matchDateTime, conditions);

            // Assert
            Assert.AreEqual(expected, actual);
            mockRulesDataSource.Verify(x => x.GetRulesAsync(It.IsAny<ContentType>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(It.IsAny<IConditionNode<ConditionType>>(), It.IsAny<IEnumerable<Condition<ConditionType>>>()), Times.AtLeastOnce());
        }

        [TestMethod]
        public async Task RulesEngine_MatchOneAsync_GivenContentTypeDateAndConditions_FetchesRulesForDayFailsEvalsAndReturnsNull()
        {
            // Arrange
            DateTime matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            ContentType contentType = ContentType.Type1;
            IEnumerable<Condition<ConditionType>> conditions = new[]
            {
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsoCountryCode,
                    Value = "BRZ"
                },
                new Condition<ConditionType>
                {
                    Type = ConditionType.IsoCurrency,
                    Value = "USD"
                }
            };

            IEnumerable<Rule<ContentType, ConditionType>> rules = new[]
            {
                new Rule<ContentType, ConditionType>
                {
                    ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                    DateBegin = new DateTime(2018, 01, 01),
                    DateEnd = new DateTime(2019, 01, 01),
                    Name = "Expected rule",
                    Priority = 3,
                    RootCondition = new StringConditionNode<ConditionType>(ConditionType.IsoCountryCode, Operators.Equal, "USA")
                },
                new Rule<ContentType, ConditionType>
                {
                    ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                    DateBegin = new DateTime(2010, 01, 01),
                    DateEnd = new DateTime(2021, 01, 01),
                    Name = "Expected rule",
                    Priority = 200,
                    RootCondition = new StringConditionNode<ConditionType>(ConditionType.IsoCountryCode, Operators.Equal, "USA")
                }
            };
            Mock<IRulesDataSource<ContentType, ConditionType>> mockRulesDataSource = SetupMockForRulesDataSource(rules);
            Mock<IConditionsEvalEngine<ConditionType>> mockConditionsEvalEngine = SetupMockForConditionsEvalEngine(false);
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.Default;

            RulesEngine<ContentType, ConditionType> sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesDataSource.Object, rulesEngineOptions);

            // Act
            Rule<ContentType, ConditionType> actual = await sut.MatchOneAsync(contentType, matchDateTime, conditions);

            // Assert
            Assert.IsNull(actual);
            mockRulesDataSource.Verify(x => x.GetRulesAsync(It.IsAny<ContentType>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(It.IsAny<IConditionNode<ConditionType>>(), It.IsAny<IEnumerable<Condition<ConditionType>>>()), Times.AtLeastOnce());
        }

        private static Mock<IConditionsEvalEngine<ConditionType>> SetupMockForConditionsEvalEngine(bool result)
        {
            Mock<IConditionsEvalEngine<ConditionType>> mockConditionsEvalEngine = new Mock<IConditionsEvalEngine<ConditionType>>();
            mockConditionsEvalEngine.Setup(x => x.Eval(It.IsAny<IConditionNode<ConditionType>>(), It.IsAny<IEnumerable<Condition<ConditionType>>>()))
                .Returns(result);
            return mockConditionsEvalEngine;
        }

        private static Mock<IRulesDataSource<ContentType, ConditionType>> SetupMockForRulesDataSource(IEnumerable<Rule<ContentType, ConditionType>> rules)
        {
            Mock<IRulesDataSource<ContentType, ConditionType>> mockRulesDataSource = new Mock<IRulesDataSource<ContentType, ConditionType>>();
            mockRulesDataSource.Setup(x => x.GetRulesAsync(It.IsAny<ContentType>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(rules);
            return mockRulesDataSource;
        }
    }
}