namespace Rules.Framework.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class RulesEngineTests
    {
        [Fact]
        public async Task MatchManyAsync_GivenContentTypeDateAndConditions_FetchesRulesForDayEvalsAndReturnsAllMatches()
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

            Rule<ContentType, ConditionType> expected1 = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Expected rule 1",
                Priority = 3,
                RootCondition = new StringConditionNode<ConditionType>(ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            Rule<ContentType, ConditionType> expected2 = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2010, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Expected rule 2",
                Priority = 200,
                RootCondition = new StringConditionNode<ConditionType>(ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            Rule<ContentType, ConditionType> notExpected = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Not expected rule",
                Priority = 1, // Topmost rule, should be the one that wins if options are set to topmost wins.
                RootCondition = new StringConditionNode<ConditionType>(ConditionType.IsoCountryCode, Operators.Equal, "CHE")
            };

            IEnumerable<Rule<ContentType, ConditionType>> rules = new[]
            {
                expected1,
                expected2,
                notExpected
            };

            EvaluationOptions evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };
            Mock<IRulesDataSource<ContentType, ConditionType>> mockRulesDataSource = SetupMockForRulesDataSource(rules);
            Mock<IConditionsEvalEngine<ConditionType>> mockConditionsEvalEngine = SetupMockForConditionsEvalEngine((rootConditionNode, inputConditions, evalOptions) =>
            {
                switch (rootConditionNode)
                {
                    case StringConditionNode<ConditionType> stringConditionNode:
                        return stringConditionNode.Operand == "USA";

                    default:
                        return false;
                }
            }, evaluationOptions);
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            RulesEngine<ContentType, ConditionType> sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesDataSource.Object, rulesEngineOptions);

            // Act
            IEnumerable<Rule<ContentType, ConditionType>> actual = await sut.MatchManyAsync(contentType, matchDateTime, conditions);

            // Assert
            actual.Should().Contain(expected1)
                .And.Contain(expected2)
                .And.NotContain(notExpected);
            mockRulesDataSource.Verify(x => x.GetRulesAsync(It.IsAny<ContentType>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode<ConditionType>>(),
                It.IsAny<IEnumerable<Condition<ConditionType>>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenContentTypeDateAndConditions_FetchesRulesForDayEvalsAndReturnsTheBottommostPriorityOne()
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

            Rule<ContentType, ConditionType> other = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Expected rule",
                Priority = 3,
                RootCondition = new StringConditionNode<ConditionType>(ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            Rule<ContentType, ConditionType> expected = new Rule<ContentType, ConditionType>
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
                other,
                expected
            };

            EvaluationOptions evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };
            Mock<IRulesDataSource<ContentType, ConditionType>> mockRulesDataSource = SetupMockForRulesDataSource(rules);
            Mock<IConditionsEvalEngine<ConditionType>> mockConditionsEvalEngine = SetupMockForConditionsEvalEngine(true, evaluationOptions);
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            rulesEngineOptions.PriotityCriteria = PriorityCriterias.BottommostRuleWins;

            RulesEngine<ContentType, ConditionType> sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesDataSource.Object, rulesEngineOptions);

            // Act
            Rule<ContentType, ConditionType> actual = await sut.MatchOneAsync(contentType, matchDateTime, conditions);

            // Assert
            actual.Should().BeSameAs(expected);
            mockRulesDataSource.Verify(x => x.GetRulesAsync(It.IsAny<ContentType>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode<ConditionType>>(),
                It.IsAny<IEnumerable<Condition<ConditionType>>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenContentTypeDateAndConditions_FetchesRulesForDayEvalsAndReturnsTheTopmostPriorityOne()
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
                expected,
                other
            };

            EvaluationOptions evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };
            Mock<IRulesDataSource<ContentType, ConditionType>> mockRulesDataSource = SetupMockForRulesDataSource(rules);
            Mock<IConditionsEvalEngine<ConditionType>> mockConditionsEvalEngine = SetupMockForConditionsEvalEngine(true, evaluationOptions);
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            RulesEngine<ContentType, ConditionType> sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesDataSource.Object, rulesEngineOptions);

            // Act
            Rule<ContentType, ConditionType> actual = await sut.MatchOneAsync(contentType, matchDateTime, conditions);

            // Assert
            actual.Should().BeSameAs(expected);
            mockRulesDataSource.Verify(x => x.GetRulesAsync(It.IsAny<ContentType>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode<ConditionType>>(),
                It.IsAny<IEnumerable<Condition<ConditionType>>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenContentTypeDateAndConditions_FetchesRulesForDayFailsEvalsAndReturnsNull()
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

            EvaluationOptions evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };
            Mock<IRulesDataSource<ContentType, ConditionType>> mockRulesDataSource = SetupMockForRulesDataSource(rules);
            Mock<IConditionsEvalEngine<ConditionType>> mockConditionsEvalEngine = SetupMockForConditionsEvalEngine(false, evaluationOptions);
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            RulesEngine<ContentType, ConditionType> sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesDataSource.Object, rulesEngineOptions);

            // Act
            Rule<ContentType, ConditionType> actual = await sut.MatchOneAsync(contentType, matchDateTime, conditions);

            // Assert
            actual.Should().BeNull();
            mockRulesDataSource.Verify(x => x.GetRulesAsync(It.IsAny<ContentType>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode<ConditionType>>(),
                It.IsAny<IEnumerable<Condition<ConditionType>>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        private static Mock<IConditionsEvalEngine<ConditionType>> SetupMockForConditionsEvalEngine(bool result, EvaluationOptions evaluationOptions)
        {
            Mock<IConditionsEvalEngine<ConditionType>> mockConditionsEvalEngine = new Mock<IConditionsEvalEngine<ConditionType>>();
            mockConditionsEvalEngine.Setup(x => x.Eval(
                    It.IsAny<IConditionNode<ConditionType>>(),
                    It.IsAny<IEnumerable<Condition<ConditionType>>>(),
                    It.Is<EvaluationOptions>(eo => eo == evaluationOptions)))
                .Returns(result);
            return mockConditionsEvalEngine;
        }

        private static Mock<IConditionsEvalEngine<ConditionType>> SetupMockForConditionsEvalEngine(Func<IConditionNode<ConditionType>, IEnumerable<Condition<ConditionType>>, EvaluationOptions, bool> evalFunc, EvaluationOptions evaluationOptions)
        {
            Mock<IConditionsEvalEngine<ConditionType>> mockConditionsEvalEngine = new Mock<IConditionsEvalEngine<ConditionType>>();
            mockConditionsEvalEngine.Setup(x => x.Eval(
                    It.IsAny<IConditionNode<ConditionType>>(),
                    It.IsAny<IEnumerable<Condition<ConditionType>>>(),
                    It.Is<EvaluationOptions>(eo => eo == evaluationOptions)))
                .Returns(evalFunc);
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