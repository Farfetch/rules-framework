namespace Rules.Framework.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FluentValidation;
    using FluentValidation.Results;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Source;
    using Rules.Framework.Tests.Stubs;
    using Rules.Framework.Validation;
    using Xunit;

    public class RulesEngineTests
    {
        private readonly Mock<IConditionsEvalEngine<ConditionType>> mockConditionsEvalEngine;
        private readonly Mock<IConditionTypeExtractor<ContentType, ConditionType>> mockCondtionTypeExtractor;
        private readonly Mock<IRulesSource<ContentType, ConditionType>> mockRulesSource;

        public RulesEngineTests()
        {
            this.mockRulesSource = new Mock<IRulesSource<ContentType, ConditionType>>();
            this.mockCondtionTypeExtractor = new Mock<IConditionTypeExtractor<ContentType, ConditionType>>();
            this.mockConditionsEvalEngine = new Mock<IConditionsEvalEngine<ConditionType>>();
        }

        [Fact]
        public async Task ActivateRuleAsync_GivenEmptyRuleDataSource_ActivatesRuleSuccessfully()
        {
            // Arrange
            var contentType = ContentType.Type1;

            var testRule = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Update test rule",
                Priority = 3,
                Active = false,
                RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            mockRulesSource.Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs<ContentType>>()))
                .ReturnsAsync(new List<Rule<ContentType, ConditionType>> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockCondtionTypeExtractor.Object);

            // Act
            var actual = await sut.ActivateRuleAsync(testRule).ConfigureAwait(false);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            mockRulesSource.Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs<ContentType>>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode<ConditionType>>(),
                It.IsAny<IDictionary<ConditionType, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        [Fact]
        public async Task AddRuleAsync_GivenEmptyRuleDataSource_AddsRuleSuccesfully()
        {
            // Arrange
            var contentType = ContentType.Type1;

            var testRule = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Test rule",
                Priority = 3,
                RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            EvaluationOptions evaluationOptions = new()
            {
                MatchMode = MatchModes.Exact
            };

            this.SetupMockForConditionsEvalEngine(true, evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            rulesEngineOptions.PriorityCriteria = PriorityCriterias.BottommostRuleWins;

            var sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockCondtionTypeExtractor.Object);

            // Act
            var actual = await sut.AddRuleAsync(testRule, RuleAddPriorityOption.AtBottom);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            mockRulesSource.Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs<ContentType>>()), Times.Never());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode<ConditionType>>(),
                It.IsAny<IDictionary<ConditionType, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        [Fact]
        public async Task DeactivateRuleAsync_GivenEmptyRuleDataSource_DeactivatesRuleSuccessfully()
        {
            // Arrange
            var contentType = ContentType.Type1;

            var testRule = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Update test rule",
                Priority = 3,
                RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            mockRulesSource.Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs<ContentType>>()))
                .ReturnsAsync(new List<Rule<ContentType, ConditionType>> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockCondtionTypeExtractor.Object);

            // Act
            var actual = await sut.DeactivateRuleAsync(testRule).ConfigureAwait(false);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            mockRulesSource.Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs<ContentType>>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode<ConditionType>>(),
                It.IsAny<IDictionary<ConditionType, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        [Fact]
        public void GetPriorityCriterias_GivenRulesEngineOptionsNewWithDefaults_ReturnsTopMostRuleWins()
        {
            // Arrange
            var rulesEngine = new RulesEngine<ContentType, ConditionType>(
                Mock.Of<IConditionsEvalEngine<ConditionType>>(),
                Mock.Of<IRulesSource<ContentType, ConditionType>>(),
                Mock.Of<IValidatorProvider>(),
                RulesEngineOptions.NewWithDefaults(),
                Mock.Of<IConditionTypeExtractor<ContentType, ConditionType>>()
                );

            //Act
            var priorityCriterias = rulesEngine.GetPriorityCriteria();

            //Arrange
            priorityCriterias.Should().Be(PriorityCriterias.TopmostRuleWins);
        }

        [Fact]
        public async Task GetUniqueConditionTypesAsync_GivenThereAreRulesInDataSource_ReturnsAllRequiredConditionTypes()
        {
            // Arrange

            var dateBegin = new DateTime(2018, 01, 01);
            var dateEnd = new DateTime(2019, 01, 01);

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            var expectedCondtionTypes = new List<ConditionType> { ConditionType.IsoCountryCode };

            mockCondtionTypeExtractor.Setup(x => x.GetConditionTypes(It.IsAny<IEnumerable<Rule<ContentType, ConditionType>>>()))
                .Returns(expectedCondtionTypes);

            this.SetupMockForConditionsEvalEngine((rootConditionNode, _, _) =>
            {
                switch (rootConditionNode)
                {
                    case ValueConditionNode<ConditionType> stringConditionNode:
                        return stringConditionNode.Operand.ToString() == "USA";

                    default:
                        return false;
                }
            }, evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockCondtionTypeExtractor.Object);

            // Act
            var actual = await sut.GetUniqueConditionTypesAsync(ContentType.Type1, dateBegin, dateEnd);

            // Assert
            actual.Should().NotBeNull();
            actual.ToList().Count.Should().Be(1);
            actual.Should().BeEquivalentTo(expectedCondtionTypes);
        }

        [Fact]
        public async Task MatchManyAsync_GivenContentTypeDateAndConditions_FetchesRulesForDayEvalsAndReturnsAllMatches()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var contentType = ContentType.Type1;
            var conditions = new[]
            {
                new Condition<ConditionType>(ConditionType.IsoCountryCode, "USA"),
                new Condition<ConditionType>(ConditionType.IsoCurrency, "USD")
            };

            var expected1 = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Expected rule 1",
                Priority = 3,
                RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            var expected2 = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2010, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Expected rule 2",
                Priority = 200,
                RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            var notExpected = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Not expected rule",
                Priority = 1, // Topmost rule, should be the one that wins if options are set to topmost wins.
                RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "CHE")
            };

            var rules = new[]
            {
                expected1,
                expected2,
                notExpected
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };
            this.SetupMockForRulesDataSource(rules);

            this.SetupMockForConditionsEvalEngine((rootConditionNode, _, _) =>
            {
                return rootConditionNode is ValueConditionNode<ConditionType> stringConditionNode && stringConditionNode.Operand.ToString() == "USA";
            }, evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockCondtionTypeExtractor.Object);

            // Act
            var actual = await sut.MatchManyAsync(contentType, matchDateTime, conditions).ConfigureAwait(false);

            // Assert
            actual.Should().Contain(expected1)
                .And.Contain(expected2)
                .And.NotContain(notExpected);
            mockRulesSource.Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs<ContentType>>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode<ConditionType>>(),
                It.IsAny<IDictionary<ConditionType, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenContentTypeDateAndConditions_FetchesRulesForDayEvalsAndReturnsTheBottommostPriorityOne()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var contentType = ContentType.Type1;
            var conditions = new[]
            {
                new Condition<ConditionType>(ConditionType.IsoCountryCode, "USA"),
                new Condition<ConditionType>(ConditionType.IsoCurrency, "USD")
            };

            var other = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Expected rule",
                Priority = 3,
                RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            var expected = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2010, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Expected rule",
                Priority = 200,
                RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            var rules = new[]
            {
                other,
                expected
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            this.SetupMockForRulesDataSource(rules);

            this.SetupMockForConditionsEvalEngine(true, evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            rulesEngineOptions.PriorityCriteria = PriorityCriterias.BottommostRuleWins;

            var sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockCondtionTypeExtractor.Object);

            // Act
            var actual = await sut.MatchOneAsync(contentType, matchDateTime, conditions);

            // Assert
            actual.Should().BeSameAs(expected);
            mockRulesSource.Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs<ContentType>>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode<ConditionType>>(),
                It.IsAny<IDictionary<ConditionType, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenContentTypeDateAndConditions_FetchesRulesForDayEvalsAndReturnsTheTopmostPriorityOne()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var contentType = ContentType.Type1;
            var conditions = new[]
            {
                new Condition<ConditionType>(ConditionType.IsoCountryCode, "USA"),
                new Condition<ConditionType>(ConditionType.IsoCurrency, "USD")
            };

            var expected = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Expected rule",
                Priority = 3,
                RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            var other = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2010, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Expected rule",
                Priority = 200,
                RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            var rules = new[]
            {
                expected,
                other
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            this.SetupMockForRulesDataSource(rules);

            this.SetupMockForConditionsEvalEngine(true, evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockCondtionTypeExtractor.Object);

            // Act
            var actual = await sut.MatchOneAsync(contentType, matchDateTime, conditions);

            // Assert
            actual.Should().BeSameAs(expected);
            mockRulesSource.Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs<ContentType>>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode<ConditionType>>(),
                It.IsAny<IDictionary<ConditionType, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenContentTypeDateAndConditions_FetchesRulesForDayFailsEvalsAndReturnsNull()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var contentType = ContentType.Type1;
            var conditions = new[]
            {
                new Condition<ConditionType>(ConditionType.IsoCountryCode, "USA"),
                new Condition<ConditionType>(ConditionType.IsoCurrency, "USD")
            };

            var rules = new[]
            {
                new Rule<ContentType, ConditionType>
                {
                    ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                    DateBegin = new DateTime(2018, 01, 01),
                    DateEnd = new DateTime(2019, 01, 01),
                    Name = "Expected rule",
                    Priority = 3,
                    RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String,ConditionType.IsoCountryCode, Operators.Equal, "USA")
                },
                new Rule<ContentType, ConditionType>
                {
                    ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                    DateBegin = new DateTime(2010, 01, 01),
                    DateEnd = new DateTime(2021, 01, 01),
                    Name = "Expected rule",
                    Priority = 200,
                    RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String,ConditionType.IsoCountryCode, Operators.Equal, "USA")
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            this.SetupMockForRulesDataSource(rules);

            this.SetupMockForConditionsEvalEngine(false, evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockCondtionTypeExtractor.Object);

            // Act
            var actual = await sut.MatchOneAsync(contentType, matchDateTime, conditions);

            // Assert
            actual.Should().BeNull();
            mockRulesSource.Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs<ContentType>>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode<ConditionType>>(),
                It.IsAny<IDictionary<ConditionType, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task SearchAsync_GivenInvalidSearchArgs_ThrowsArgumentException()
        {
            // Arrange
            var contentType = ContentType.Type1;
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var searchArgs = new SearchArgs<ContentType, ConditionType>(contentType, matchDateTime, matchDateTime);

            var rules = new[]
            {
                new Rule<ContentType, ConditionType>
                {
                    ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                    DateBegin = new DateTime(2018, 01, 01),
                    DateEnd = new DateTime(2019, 01, 01),
                    Name = "Expected rule",
                    Priority = 3,
                    RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String,ConditionType.IsoCountryCode, Operators.Equal, "USA")
                },
                new Rule<ContentType, ConditionType>
                {
                    ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                    DateBegin = new DateTime(2010, 01, 01),
                    DateEnd = new DateTime(2021, 01, 01),
                    Name = "Expected rule",
                    Priority = 200,
                    RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String,ConditionType.IsoCountryCode, Operators.Equal, "USA")
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            this.SetupMockForRulesDataSource(rules);

            this.SetupMockForConditionsEvalEngine(false, evaluationOptions);

            var validator = Mock.Of<IValidator<SearchArgs<ContentType, ConditionType>>>();
            Mock.Get(validator)
                .Setup(x => x.ValidateAsync(It.IsAny<SearchArgs<ContentType, ConditionType>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Prop1", "Sample error message") }));

            var validatorProvider = Mock.Of<IValidatorProvider>();
            Mock.Get(validatorProvider)
                .Setup(x => x.GetValidatorFor<SearchArgs<ContentType, ConditionType>>())
                .Returns(validator);
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockCondtionTypeExtractor.Object);

            // Act
            var argumentException = await Assert.ThrowsAsync<ArgumentException>(() => sut.SearchAsync(searchArgs)).ConfigureAwait(false);

            // Assert
            argumentException.Should().NotBeNull();
            argumentException.ParamName.Should().Be(nameof(searchArgs));
            argumentException.Message.Should().StartWith($"Specified '{nameof(searchArgs)}' with invalid search values:");
        }

        [Fact]
        public async Task SearchAsync_GivenNullSearchArgs_ThrowsArgumentNullException()
        {
            // Arrange
            SearchArgs<ContentType, ConditionType> searchArgs = null;
            var contentType = ContentType.Type1;

            var rules = new[]
            {
                new Rule<ContentType, ConditionType>
                {
                    ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                    DateBegin = new DateTime(2018, 01, 01),
                    DateEnd = new DateTime(2019, 01, 01),
                    Name = "Expected rule",
                    Priority = 3,
                    RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String,ConditionType.IsoCountryCode, Operators.Equal, "USA")
                },
                new Rule<ContentType, ConditionType>
                {
                    ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                    DateBegin = new DateTime(2010, 01, 01),
                    DateEnd = new DateTime(2021, 01, 01),
                    Name = "Expected rule",
                    Priority = 200,
                    RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String,ConditionType.IsoCountryCode, Operators.Equal, "USA")
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            this.SetupMockForRulesDataSource(rules);

            this.SetupMockForConditionsEvalEngine(false, evaluationOptions);
            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockCondtionTypeExtractor.Object);

            // Act
            var argumentNullException = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.SearchAsync(searchArgs)).ConfigureAwait(false);

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(searchArgs));
        }

        [Fact]
        public async Task UpdateRuleAsync_GivenEmptyRuleDataSource_UpdatesRuleSuccesfully()
        {
            // Arrange
            var contentType = ContentType.Type1;

            var testRule = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Update test rule",
                Priority = 3,
                RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            mockRulesSource.Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs<ContentType>>()))
                .ReturnsAsync(new List<Rule<ContentType, ConditionType>> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockCondtionTypeExtractor.Object);

            testRule.DateEnd = new DateTime(2019, 01, 02);
            testRule.Priority = 1;

            // Act
            var actual = await sut.UpdateRuleAsync(testRule).ConfigureAwait(false);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            mockRulesSource.Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs<ContentType>>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode<ConditionType>>(),
                It.IsAny<IDictionary<ConditionType, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        [Fact]
        public async Task UpdateRuleAsync_GivenRuleWithInvalidDateEnd_UpdatesRuleFailure()
        {
            // Arrange
            var contentType = ContentType.Type1;

            var testRule = new Rule<ContentType, ConditionType>
            {
                ContentContainer = new ContentContainer<ContentType>(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Update test rule",
                Priority = 3,
                RootCondition = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "USA")
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            mockRulesSource.Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs<ContentType>>()))
                .ReturnsAsync(new List<Rule<ContentType, ConditionType>> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine<ContentType, ConditionType>(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockCondtionTypeExtractor.Object);

            testRule.DateEnd = testRule.DateBegin.AddYears(-2);
            testRule.Priority = 1;

            // Act
            var actual = await sut.UpdateRuleAsync(testRule).ConfigureAwait(false);

            // Assert
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().NotBeEmpty();

            mockRulesSource.Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs<ContentType>>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode<ConditionType>>(),
                It.IsAny<IDictionary<ConditionType, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        private void SetupMockForConditionsEvalEngine(Func<IConditionNode<ConditionType>, IDictionary<ConditionType, object>, EvaluationOptions, bool> evalFunc, EvaluationOptions evaluationOptions)
        {
            this.mockConditionsEvalEngine.Setup(x => x.Eval(
                    It.IsAny<IConditionNode<ConditionType>>(),
                    It.IsAny<IDictionary<ConditionType, object>>(),
                    It.Is<EvaluationOptions>(eo => eo == evaluationOptions)))
                .Returns(evalFunc);
        }

        private void SetupMockForConditionsEvalEngine(bool result, EvaluationOptions evaluationOptions)
        {
            this.mockConditionsEvalEngine.Setup(x => x.Eval(
                    It.IsAny<IConditionNode<ConditionType>>(),
                    It.IsAny<IDictionary<ConditionType, object>>(),
                    It.Is<EvaluationOptions>(eo => eo == evaluationOptions)))
                .Returns(result);
        }

        private void SetupMockForRulesDataSource(IEnumerable<Rule<ContentType, ConditionType>> rules)
        {
            this.mockRulesSource.Setup(x => x.GetRulesAsync(It.IsAny<GetRulesArgs<ContentType>>()))
                .ReturnsAsync(rules);
        }
    }
}