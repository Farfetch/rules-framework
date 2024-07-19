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
    using Rules.Framework;
    using Rules.Framework.ConditionNodes;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Source;
    using Rules.Framework.Tests.Stubs;
    using Rules.Framework.Validation;
    using Xunit;

    public class RulesEngineTests
    {
        private readonly Mock<IConditionsEvalEngine> mockConditionsEvalEngine;
        private readonly Mock<IConditionTypeExtractor> mockConditionTypeExtractor;
        private readonly Mock<IRulesSource> mockRulesSource;

        public RulesEngineTests()
        {
            this.mockRulesSource = new Mock<IRulesSource>();
            this.mockConditionTypeExtractor = new Mock<IConditionTypeExtractor>();
            this.mockConditionsEvalEngine = new Mock<IConditionsEvalEngine>();
        }

        [Fact]
        public async Task ActivateRuleAsync_GivenEmptyRuleDataSource_ActivatesRuleSuccessfully()
        {
            // Arrange
            var contentType = ContentType.Type1.ToString();

            var testRule = new Rule
            {
                ContentContainer = new ContentContainer(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Update test rule",
                Priority = 3,
                Active = false,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            mockRulesSource.Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync(new List<Rule> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockConditionTypeExtractor.Object);

            // Act
            var actual = await sut.ActivateRuleAsync(testRule);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            mockRulesSource.Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        [Fact]
        public async Task AddRuleAsync_GivenEmptyRuleDataSource_AddsRuleSuccesfully()
        {
            // Arrange
            var contentType = ContentType.Type1.ToString();

            var testRule = new Rule
            {
                ContentContainer = new ContentContainer(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Test rule",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            EvaluationOptions evaluationOptions = new()
            {
                MatchMode = MatchModes.Exact
            };

            this.SetupMockForConditionsEvalEngine(true, evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            rulesEngineOptions.PriorityCriteria = PriorityCriterias.BottommostRuleWins;

            var sut = new RulesEngine(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockConditionTypeExtractor.Object);

            // Act
            var actual = await sut.AddRuleAsync(testRule, RuleAddPriorityOption.AtBottom);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            mockRulesSource.Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()), Times.Never());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        [Fact]
        public async Task DeactivateRuleAsync_GivenEmptyRuleDataSource_DeactivatesRuleSuccessfully()
        {
            // Arrange
            var contentType = ContentType.Type1.ToString();

            var testRule = new Rule
            {
                ContentContainer = new ContentContainer(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Update test rule",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            mockRulesSource.Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync(new List<Rule> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockConditionTypeExtractor.Object);

            // Act
            var actual = await sut.DeactivateRuleAsync(testRule);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            mockRulesSource.Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
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

            var expectedCondtionTypes = new List<string> { ConditionType.IsoCountryCode.ToString() };

            mockConditionTypeExtractor.Setup(x => x.GetConditionTypes(It.IsAny<IEnumerable<Rule>>()))
                .Returns(expectedCondtionTypes);

            this.SetupMockForConditionsEvalEngine((rootConditionNode, _, _) => rootConditionNode switch
            {
                ValueConditionNode stringConditionNode => stringConditionNode.Operand.ToString() == "USA",
                _ => false,
            }, evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockConditionTypeExtractor.Object);

            // Act
            var actual = await sut.GetUniqueConditionTypesAsync(ContentType.Type1.ToString(), dateBegin, dateEnd);

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
            var contentType = ContentType.Type1.ToString();
            var conditions = new[]
            {
                new Condition<string>(ConditionType.IsoCountryCode.ToString(), "USA"),
                new Condition<string>(ConditionType.IsoCurrency.ToString(), "USD")
            };

            var expected1 = new Rule
            {
                ContentContainer = new ContentContainer(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Expected rule 1",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var expected2 = new Rule
            {
                ContentContainer = new ContentContainer(contentType, (t) => new object()),
                DateBegin = new DateTime(2010, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Expected rule 2",
                Priority = 200,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var notExpected = new Rule
            {
                ContentContainer = new ContentContainer(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Not expected rule",
                Priority = 1, // Topmost rule, should be the one that wins if options are set to topmost wins.
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "CHE")
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
                return rootConditionNode is ValueConditionNode stringConditionNode && stringConditionNode.Operand.ToString() == "USA";
            }, evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockConditionTypeExtractor.Object);

            // Act
            var actual = await sut.MatchManyAsync(contentType, matchDateTime, conditions);

            // Assert
            actual.Should().Contain(expected1)
                .And.Contain(expected2)
                .And.NotContain(notExpected);
            mockRulesSource.Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenContentTypeDateAndConditions_FetchesRulesForDayEvalsAndReturnsTheBottommostPriorityOne()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var contentType = ContentType.Type1.ToString();
            var conditions = new[]
            {
                new Condition<string>(ConditionType.IsoCountryCode.ToString(), "USA"),
                new Condition<string>(ConditionType.IsoCurrency.ToString(), "USD")
            };

            var other = new Rule
            {
                ContentContainer = new ContentContainer(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Expected rule",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var expected = new Rule
            {
                ContentContainer = new ContentContainer(contentType, (t) => new object()),
                DateBegin = new DateTime(2010, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Expected rule",
                Priority = 200,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
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

            var sut = new RulesEngine(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockConditionTypeExtractor.Object);

            // Act
            var actual = await sut.MatchOneAsync(contentType, matchDateTime, conditions);

            // Assert
            actual.Should().BeSameAs(expected);
            mockRulesSource.Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenContentTypeDateAndConditions_FetchesRulesForDayEvalsAndReturnsTheTopmostPriorityOne()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var contentType = ContentType.Type1.ToString();
            var conditions = new[]
            {
                new Condition<string>(ConditionType.IsoCountryCode.ToString(), "USA"),
                new Condition<string>(ConditionType.IsoCurrency.ToString(), "USD")
            };

            var expected = new Rule
            {
                ContentContainer = new ContentContainer(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Expected rule",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var other = new Rule
            {
                ContentContainer = new ContentContainer(contentType, (t) => new object()),
                DateBegin = new DateTime(2010, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Expected rule",
                Priority = 200,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
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

            var sut = new RulesEngine(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockConditionTypeExtractor.Object);

            // Act
            var actual = await sut.MatchOneAsync(contentType, matchDateTime, conditions);

            // Assert
            actual.Should().BeSameAs(expected);
            mockRulesSource.Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenContentTypeDateAndConditions_FetchesRulesForDayFailsEvalsAndReturnsNull()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var contentType = ContentType.Type1.ToString();
            var conditions = new[]
            {
                new Condition<string>(ConditionType.IsoCountryCode.ToString(), "USA"),
                new Condition<string>(ConditionType.IsoCurrency.ToString(), "USD")
            };

            var rules = new[]
            {
                new Rule
                {
                    ContentContainer = new ContentContainer(contentType, (t) => new object()),
                    DateBegin = new DateTime(2018, 01, 01),
                    DateEnd = new DateTime(2019, 01, 01),
                    Name = "Expected rule",
                    Priority = 3,
                    RootCondition = new ValueConditionNode(DataTypes.String,ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
                },
                new Rule
                {
                    ContentContainer = new ContentContainer(contentType, (t) => new object()),
                    DateBegin = new DateTime(2010, 01, 01),
                    DateEnd = new DateTime(2021, 01, 01),
                    Name = "Expected rule",
                    Priority = 200,
                    RootCondition = new ValueConditionNode(DataTypes.String,ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
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

            var sut = new RulesEngine(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockConditionTypeExtractor.Object);

            // Act
            var actual = await sut.MatchOneAsync(contentType, matchDateTime, conditions);

            // Assert
            actual.Should().BeNull();
            mockRulesSource.Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task SearchAsync_GivenInvalidSearchArgs_ThrowsArgumentException()
        {
            // Arrange
            var contentType = ContentType.Type1.ToString();
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var searchArgs = new SearchArgs<string, string>(contentType, matchDateTime, matchDateTime);

            var rules = new[]
            {
                new Rule
                {
                    ContentContainer = new ContentContainer(contentType, (t) => new object()),
                    DateBegin = new DateTime(2018, 01, 01),
                    DateEnd = new DateTime(2019, 01, 01),
                    Name = "Expected rule",
                    Priority = 3,
                    RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
                },
                new Rule
                {
                    ContentContainer = new ContentContainer(contentType, (t) => new object()),
                    DateBegin = new DateTime(2010, 01, 01),
                    DateEnd = new DateTime(2021, 01, 01),
                    Name = "Expected rule",
                    Priority = 200,
                    RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            this.SetupMockForRulesDataSource(rules);

            this.SetupMockForConditionsEvalEngine(false, evaluationOptions);

            var validator = Mock.Of<IValidator<SearchArgs<string, string>>>();
            Mock.Get(validator)
                .Setup(x => x.ValidateAsync(It.IsAny<SearchArgs<string, string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Prop1", "Sample error message") }));

            var validatorProvider = Mock.Of<IValidatorProvider>();
            Mock.Get(validatorProvider)
                .Setup(x => x.GetValidatorFor<SearchArgs<string, string>>())
                .Returns(validator);
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockConditionTypeExtractor.Object);

            // Act
            var argumentException = await Assert.ThrowsAsync<ArgumentException>(() => sut.SearchAsync(searchArgs));

            // Assert
            argumentException.Should().NotBeNull();
            argumentException.ParamName.Should().Be(nameof(searchArgs));
            argumentException.Message.Should().StartWith($"Specified '{nameof(searchArgs)}' with invalid search values:");
        }

        [Fact]
        public async Task SearchAsync_GivenNullSearchArgs_ThrowsArgumentNullException()
        {
            // Arrange
            SearchArgs<string, string> searchArgs = null;
            var contentType = ContentType.Type1.ToString();

            var rules = new[]
            {
                new Rule
                {
                    ContentContainer = new ContentContainer(contentType, (t) => new object()),
                    DateBegin = new DateTime(2018, 01, 01),
                    DateEnd = new DateTime(2019, 01, 01),
                    Name = "Expected rule",
                    Priority = 3,
                    RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
                },
                new Rule
                {
                    ContentContainer = new ContentContainer(contentType, (t) => new object()),
                    DateBegin = new DateTime(2010, 01, 01),
                    DateEnd = new DateTime(2021, 01, 01),
                    Name = "Expected rule",
                    Priority = 200,
                    RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
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

            var sut = new RulesEngine(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockConditionTypeExtractor.Object);

            // Act
            var argumentNullException = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.SearchAsync(searchArgs));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(searchArgs));
        }

        [Fact]
        public async Task UpdateRuleAsync_GivenEmptyRuleDataSource_UpdatesRuleSuccesfully()
        {
            // Arrange
            var contentType = ContentType.Type1.ToString();

            var testRule = new Rule
            {
                ContentContainer = new ContentContainer(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Update test rule",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            mockRulesSource.Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync(new List<Rule> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockConditionTypeExtractor.Object);

            testRule.DateEnd = new DateTime(2019, 01, 02);
            testRule.Priority = 1;

            // Act
            var actual = await sut.UpdateRuleAsync(testRule);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            mockRulesSource.Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        [Fact]
        public async Task UpdateRuleAsync_GivenRuleWithInvalidDateEnd_UpdatesRuleFailure()
        {
            // Arrange
            var contentType = ContentType.Type1.ToString();

            var testRule = new Rule
            {
                ContentContainer = new ContentContainer(contentType, (t) => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Update test rule",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionType.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            mockRulesSource.Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync(new List<Rule> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine(mockConditionsEvalEngine.Object, mockRulesSource.Object, validatorProvider, rulesEngineOptions, mockConditionTypeExtractor.Object);

            testRule.DateEnd = testRule.DateBegin.AddYears(-2);
            testRule.Priority = 1;

            // Act
            var actual = await sut.UpdateRuleAsync(testRule);

            // Assert
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().NotBeEmpty();

            mockRulesSource.Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            mockConditionsEvalEngine.Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        private void SetupMockForConditionsEvalEngine(Func<IConditionNode, IDictionary<string, object>, EvaluationOptions, bool> evalFunc, EvaluationOptions evaluationOptions)
        {
            this.mockConditionsEvalEngine.Setup(x => x.Eval(
                    It.IsAny<IConditionNode>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.Is<EvaluationOptions>(eo => eo == evaluationOptions)))
                .Returns(evalFunc);
        }

        private void SetupMockForConditionsEvalEngine(bool result, EvaluationOptions evaluationOptions)
        {
            this.mockConditionsEvalEngine.Setup(x => x.Eval(
                    It.IsAny<IConditionNode>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.Is<EvaluationOptions>(eo => eo == evaluationOptions)))
                .Returns(result);
        }

        private void SetupMockForRulesDataSource(IEnumerable<Rule> rules)
        {
            this.mockRulesSource.Setup(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()))
                .ReturnsAsync(rules);
        }
    }
}