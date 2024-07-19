namespace Rules.Framework.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
        private readonly IConditionsEvalEngine conditionsEvalEngineMock;
        private readonly IRuleConditionsExtractor conditionTypeExtractorMock;
        private readonly IRulesSource rulesSourceMock;
        private readonly IValidatorProvider validatorProviderMock;

        public RulesEngineTests()
        {
            this.rulesSourceMock = Mock.Of<IRulesSource>();
            this.conditionTypeExtractorMock = Mock.Of<IRuleConditionsExtractor>();
            this.conditionsEvalEngineMock = Mock.Of<IConditionsEvalEngine>();
            this.validatorProviderMock = Mock.Of<IValidatorProvider>();
        }

        [Fact]
        public async Task ActivateRuleAsync_GivenEmptyRuleDataSource_ActivatesRuleSuccessfully()
        {
            // Arrange
            var ruleset = nameof(RulesetNames.Type1);

            var testRule = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Update test rule",
                Priority = 3,
                Active = false,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
                Ruleset = ruleset,
            };

            Mock.Get(rulesSourceMock)
                .Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync(new List<Rule> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProvider, rulesEngineOptions, conditionTypeExtractorMock);

            // Act
            var actual = await sut.ActivateRuleAsync(testRule);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task AddRuleAsync_GivenEmptyRuleDataSourceAndExistentRuleset_AddsRuleSuccessfully()
        {
            // Arrange
            var ruleset = new Ruleset(RulesetNames.Type1.ToString(), DateTime.UtcNow);

            var testRule = new Rule
            {
                Ruleset = ruleset.Name,
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Test rule",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            Mock.Get(rulesSourceMock)
                .Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()))
                .ReturnsAsync(new[] { ruleset });

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            rulesEngineOptions.PriorityCriteria = PriorityCriterias.BottommostRuleWins;

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProviderMock, rulesEngineOptions, conditionTypeExtractorMock);

            // Act
            var actual = await sut.AddRuleAsync(testRule, RuleAddPriorityOption.AtBottom);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task AddRuleAsync_GivenEmptyRuleDataSourceAndNonExistentRulesetAndAutoCreateRulesetDisabled_DoesNotAddRuleAndReportsError()
        {
            // Arrange
            var ruleset = new Ruleset(RulesetNames.Type1.ToString(), DateTime.UtcNow);

            var testRule = new Rule
            {
                Ruleset = ruleset.Name,
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Test rule",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            Mock.Get(rulesSourceMock)
                .Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()))
                .ReturnsAsync(Array.Empty<Ruleset>());

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            rulesEngineOptions.PriorityCriteria = PriorityCriterias.BottommostRuleWins;

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProviderMock, rulesEngineOptions, conditionTypeExtractorMock);

            // Act
            var actual = await sut.AddRuleAsync(testRule, RuleAddPriorityOption.AtBottom);

            // Assert
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().HaveCount(1);

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Never());
            Mock.Get(conditionsEvalEngineMock).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task AddRuleAsync_GivenEmptyRuleDataSourceAndNonExistentRulesetAndAutoCreateRulesetEnabled_CreatesRulesetAndAddsRuleSuccessfully()
        {
            // Arrange
            var ruleset = new Ruleset(RulesetNames.Type1.ToString(), DateTime.UtcNow);

            var testRule = new Rule
            {
                Ruleset = ruleset.Name,
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Test rule",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            Mock.Get(this.rulesSourceMock)
                .Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()))
                .ReturnsAsync(Array.Empty<Ruleset>());
            Mock.Get(this.rulesSourceMock)
                .Setup(x => x.CreateRulesetAsync(It.IsAny<CreateRulesetArgs>()))
                .Returns(Task.CompletedTask);

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.PriorityCriteria = PriorityCriterias.BottommostRuleWins;
            rulesEngineOptions.AutoCreateRulesets = true;

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProviderMock, rulesEngineOptions, conditionTypeExtractorMock);

            // Act
            var actual = await sut.AddRuleAsync(testRule, RuleAddPriorityOption.AtBottom);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()), Times.Once());
            Mock.Get(rulesSourceMock).Verify(x => x.CreateRulesetAsync(It.IsAny<CreateRulesetArgs>()), Times.Once());
            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateRulesetAsync_GivenExistentRulesetName_DoesNotAddRulesetToRulesSource()
        {
            // Arrange
            var contentType = RulesetNames.Type1.ToString();

            Mock.Get(this.rulesSourceMock)
                .Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()))
                .ReturnsAsync(new[] { new Ruleset(nameof(RulesetNames.Type1), DateTime.UtcNow), });

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            rulesEngineOptions.PriorityCriteria = PriorityCriterias.BottommostRuleWins;

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProviderMock, rulesEngineOptions, conditionTypeExtractorMock);

            // Act
            var operationResult = await sut.CreateRulesetAsync(contentType);

            // Assert
            operationResult.Should().NotBeNull();
            operationResult.IsSuccess.Should().BeFalse();
            operationResult.Errors.Should().NotBeNull()
                .And.HaveCount(1);

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()), Times.Once());
            Mock.Get(rulesSourceMock).Verify(x => x.CreateRulesetAsync(It.Is<CreateRulesetArgs>(x => string.Equals(x.Name, contentType))), Times.Never());
            Mock.Get(conditionsEvalEngineMock).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateRulesetAsync_GivenNonExistentRulesetName_AddsRulesetToRulesSource()
        {
            // Arrange
            var contentType = RulesetNames.Type1.ToString();

            Mock.Get(this.rulesSourceMock)
                .Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()))
                .ReturnsAsync(Array.Empty<Ruleset>());
            Mock.Get(rulesSourceMock)
                .Setup(x => x.CreateRulesetAsync(It.Is<CreateRulesetArgs>(x => string.Equals(x.Name, contentType))))
                .Returns(Task.CompletedTask);

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            rulesEngineOptions.PriorityCriteria = PriorityCriterias.BottommostRuleWins;

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProviderMock, rulesEngineOptions, conditionTypeExtractorMock);

            // Act
            var operationResult = await sut.CreateRulesetAsync(contentType);

            // Assert
            operationResult.Should().NotBeNull();
            operationResult.IsSuccess.Should().BeTrue();
            operationResult.Errors.Should().NotBeNull()
                .And.BeEmpty();

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()), Times.Once());
            Mock.Get(rulesSourceMock).Verify(x => x.CreateRulesetAsync(It.Is<CreateRulesetArgs>(x => string.Equals(x.Name, contentType))), Times.Once());
            Mock.Get(conditionsEvalEngineMock).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task DeactivateRuleAsync_GivenEmptyRuleDataSource_DeactivatesRuleSuccessfully()
        {
            // Arrange
            var ruleset = nameof(RulesetNames.Type1);

            var testRule = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Update test rule",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
                Ruleset = ruleset,
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            Mock.Get(rulesSourceMock).Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync(new List<Rule> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProvider, rulesEngineOptions, conditionTypeExtractorMock);

            // Act
            var actual = await sut.DeactivateRuleAsync(testRule);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        [Fact]
        public async Task GetRulesetsAsync_NoConditionsGiven_ReturnsRulesetsFromRulesSource()
        {
            // Arrange
            Mock.Get(this.rulesSourceMock)
                .Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()))
                .ReturnsAsync(new[] { new Ruleset(nameof(RulesetNames.Type1), DateTime.UtcNow), new Ruleset(nameof(RulesetNames.Type2), DateTime.UtcNow), });
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            rulesEngineOptions.PriorityCriteria = PriorityCriterias.BottommostRuleWins;

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProviderMock, rulesEngineOptions, conditionTypeExtractorMock);

            // Act
            var contentTypes = await sut.GetRulesetsAsync();

            // Assert
            contentTypes.Should().NotBeNull()
                .And.HaveCount(2)
                .And.Contain(r => string.Equals(r.Name, nameof(RulesetNames.Type1), StringComparison.Ordinal))
                .And.Contain(r => string.Equals(r.Name, nameof(RulesetNames.Type2), StringComparison.Ordinal));

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetUniqueConditionsAsync_GivenThereAreRulesInDataSource_ReturnsAllRequiredConditions()
        {
            // Arrange

            var dateBegin = new DateTime(2018, 01, 01);
            var dateEnd = new DateTime(2019, 01, 01);

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            var expectedConditions = new List<string> { ConditionNames.IsoCountryCode.ToString() };

            Mock.Get(conditionTypeExtractorMock)
                .Setup(x => x.GetConditions(It.IsAny<IEnumerable<Rule>>()))
                .Returns(expectedConditions);

            this.SetupMockForConditionsEvalEngine(
                (rootConditionNode, _, _) => rootConditionNode is ValueConditionNode stringConditionNode && stringConditionNode.Operand.ToString() == "USA",
                evaluationOptions);

            var validatorProvider = Mock.Of<IValidatorProvider>();

            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProvider, rulesEngineOptions, conditionTypeExtractorMock);

            // Act
            var actual = await sut.GetUniqueConditionsAsync(RulesetNames.Type1.ToString(), dateBegin, dateEnd);

            // Assert
            actual.Should().NotBeNull();
            actual.ToList().Count.Should().Be(1);
            actual.Should().BeEquivalentTo(expectedConditions);
        }

        [Fact]
        public async Task MatchManyAsync_GivenRulesetDateAndConditions_FetchesRulesForDayEvalsAndReturnsAllMatches()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var ruleset = RulesetNames.Type1.ToString();
            var conditions = new[]
            {
                new Condition<string>(ConditionNames.IsoCountryCode.ToString(), "USA"),
                new Condition<string>(ConditionNames.IsoCurrency.ToString(), "USD")
            };

            var expected1 = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Expected rule 1",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var expected2 = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2010, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Expected rule 2",
                Priority = 200,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var notExpected = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Not expected rule",
                Priority = 1, // Topmost rule, should be the one that wins if options are set to topmost wins.
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "CHE")
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

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProvider, rulesEngineOptions, conditionTypeExtractorMock);

            // Act
            var actual = await sut.MatchManyAsync(ruleset, matchDateTime, conditions);

            // Assert
            actual.Should().Contain(expected1)
                .And.Contain(expected2)
                .And.NotContain(notExpected);
            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenRulesetDateAndConditions_FetchesRulesForDayEvalsAndReturnsTheBottommostPriorityOne()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var ruleset = RulesetNames.Type1.ToString();
            var conditions = new[]
            {
                new Condition<string>(ConditionNames.IsoCountryCode.ToString(), "USA"),
                new Condition<string>(ConditionNames.IsoCurrency.ToString(), "USD")
            };

            var other = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Expected rule",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var expected = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2010, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Expected rule",
                Priority = 200,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA")
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

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProvider, rulesEngineOptions, conditionTypeExtractorMock);

            // Act
            var actual = await sut.MatchOneAsync(ruleset, matchDateTime, conditions);

            // Assert
            actual.Should().BeSameAs(expected);
            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenRulesetDateAndConditions_FetchesRulesForDayEvalsAndReturnsTheTopmostPriorityOne()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var ruleset = RulesetNames.Type1.ToString();
            var conditions = new[]
            {
                new Condition<string>(ConditionNames.IsoCountryCode.ToString(), "USA"),
                new Condition<string>(ConditionNames.IsoCurrency.ToString(), "USD")
            };

            var expected = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Expected rule",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var other = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2010, 01, 01),
                DateEnd = new DateTime(2021, 01, 01),
                Name = "Expected rule",
                Priority = 200,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA")
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

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProvider, rulesEngineOptions, conditionTypeExtractorMock);

            // Act
            var actual = await sut.MatchOneAsync(ruleset, matchDateTime, conditions);

            // Assert
            actual.Should().BeSameAs(expected);
            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task MatchOneAsync_GivenRulesetDateAndConditions_FetchesRulesForDayFailsEvalsAndReturnsNull()
        {
            // Arrange
            var matchDateTime = new DateTime(2018, 07, 01, 18, 19, 30);
            var ruleset = RulesetNames.Type1.ToString();
            var conditions = new[]
            {
                new Condition<string>(ConditionNames.IsoCountryCode.ToString(), "USA"),
                new Condition<string>(ConditionNames.IsoCurrency.ToString(), "USD")
            };

            var rules = new[]
            {
                new Rule
                {
                    ContentContainer = new ContentContainer(_ => new object()),
                    DateBegin = new DateTime(2018, 01, 01),
                    DateEnd = new DateTime(2019, 01, 01),
                    Name = "Expected rule",
                    Priority = 3,
                    RootCondition = new ValueConditionNode(DataTypes.String,ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA")
                },
                new Rule
                {
                    ContentContainer = new ContentContainer(_ => new object()),
                    DateBegin = new DateTime(2010, 01, 01),
                    DateEnd = new DateTime(2021, 01, 01),
                    Name = "Expected rule",
                    Priority = 200,
                    RootCondition = new ValueConditionNode(DataTypes.String,ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA")
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

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProvider, rulesEngineOptions, conditionTypeExtractorMock);

            // Act
            var actual = await sut.MatchOneAsync(ruleset, matchDateTime, conditions);

            // Assert
            actual.Should().BeNull();
            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.AtLeastOnce());
        }

        [Fact]
        public async Task UpdateRuleAsync_GivenEmptyRuleDataSource_UpdatesRuleSuccesfully()
        {
            // Arrange
            var ruleset = nameof(RulesetNames.Type1);

            var testRule = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Update test rule",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
                Ruleset = ruleset,
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            Mock.Get(rulesSourceMock).Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync(new List<Rule> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProvider, rulesEngineOptions, conditionTypeExtractorMock);

            testRule.DateEnd = new DateTime(2019, 01, 02);
            testRule.Priority = 1;

            // Act
            var actual = await sut.UpdateRuleAsync(testRule);

            // Assert
            actual.IsSuccess.Should().BeTrue();
            actual.Errors.Should().BeEmpty();

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        [Fact]
        public async Task UpdateRuleAsync_GivenRuleWithInvalidDateEnd_UpdatesRuleFailure()
        {
            // Arrange
            var testRule = new Rule
            {
                ContentContainer = new ContentContainer(_ => new object()),
                DateBegin = new DateTime(2018, 01, 01),
                DateEnd = new DateTime(2019, 01, 01),
                Name = "Update test rule",
                Priority = 3,
                RootCondition = new ValueConditionNode(DataTypes.String, ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA")
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            Mock.Get(rulesSourceMock).Setup(s => s.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync(new List<Rule> { testRule });

            var validatorProvider = Mock.Of<IValidatorProvider>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProvider, rulesEngineOptions, conditionTypeExtractorMock);

            testRule.DateEnd = testRule.DateBegin.AddYears(-2);
            testRule.Priority = 1;

            // Act
            var actual = await sut.UpdateRuleAsync(testRule);

            // Assert
            actual.IsSuccess.Should().BeFalse();
            actual.Errors.Should().NotBeEmpty();

            Mock.Get(rulesSourceMock).Verify(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()), Times.Once());
            Mock.Get(conditionsEvalEngineMock).Verify(x => x.Eval(
                It.IsAny<IConditionNode>(),
                It.IsAny<IDictionary<string, object>>(),
                It.Is<EvaluationOptions>(eo => eo == evaluationOptions)), Times.Never());
        }

        [Theory]
        [InlineData(nameof(RulesEngine.ActivateRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.AddRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.AddRuleAsync), "ruleAddPriorityOption", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.CreateRulesetAsync), "ruleset", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.DeactivateRuleAsync), "rule", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.GetUniqueConditionsAsync), "ruleset", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.MatchManyAsync), "ruleset", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.MatchOneAsync), "ruleset", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.SearchAsync), "searchArgs", typeof(ArgumentNullException))]
        [InlineData(nameof(RulesEngine.SearchAsync), "searchArgs", typeof(ArgumentException))]
        [InlineData(nameof(RulesEngine.UpdateRuleAsync), "rule", typeof(ArgumentNullException))]
        public async Task VerifyParameters_GivenNullParameter_ThrowsArgumentNullException(string methodName, string parameterName, Type exceptionType)
        {
            // Arrange
            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            var validator = Mock.Of<IValidator<SearchArgs<string, string>>>();
            Mock.Get(validator)
                .Setup(x => x.ValidateAsync(It.IsAny<SearchArgs<string, string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Prop1", "Sample error message") }));
            var validatorProvider = Mock.Of<IValidatorProvider>();
            Mock.Get(validatorProvider)
                .Setup(x => x.GetValidatorFor<SearchArgs<string, string>>())
                .Returns(validator);
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new RulesEngine(conditionsEvalEngineMock, rulesSourceMock, validatorProvider, rulesEngineOptions, conditionTypeExtractorMock);

            // Act
            var actual = await Assert.ThrowsAsync(exceptionType, async () =>
            {
                switch (methodName)
                {
                    case nameof(RulesEngine.ActivateRuleAsync):
                        _ = await sut.ActivateRuleAsync(null);
                        break;

                    case nameof(RulesEngine.AddRuleAsync):
                        switch (parameterName)
                        {
                            case "rule":
                                _ = await sut.AddRuleAsync(null, RuleAddPriorityOption.AtTop);
                                break;

                            case "ruleAddPriorityOption":
                                _ = await sut.AddRuleAsync(CreateTestStubRule(), null);
                                break;

                            default:
                                Assert.Fail("Test scenario not supported, please review test implementation to support it.");
                                break;
                        }
                        break;

                    case nameof(RulesEngine.CreateRulesetAsync):
                        await sut.CreateRulesetAsync(null);
                        break;

                    case nameof(RulesEngine.DeactivateRuleAsync):
                        _ = await sut.DeactivateRuleAsync(null);
                        break;

                    case nameof(RulesEngine.GetUniqueConditionsAsync):
                        _ = await sut.GetUniqueConditionsAsync(null, DateTime.MinValue, DateTime.MaxValue);
                        break;

                    case nameof(RulesEngine.MatchManyAsync):
                        _ = await sut.MatchManyAsync(null, DateTime.UtcNow, Enumerable.Empty<Condition<string>>());
                        break;

                    case nameof(RulesEngine.MatchOneAsync):
                        _ = await sut.MatchOneAsync(null, DateTime.UtcNow, Enumerable.Empty<Condition<string>>());
                        break;

                    case nameof(RulesEngine.SearchAsync):
                        switch (exceptionType.Name)
                        {
                            case nameof(ArgumentNullException):
                                _ = await sut.SearchAsync(null);
                                break;

                            case nameof(ArgumentException):
                                _ = await sut.SearchAsync(new SearchArgs<string, string>("test", DateTime.MinValue, DateTime.MaxValue));
                                break;

                            default:
                                Assert.Fail("Test scenario not supported, please review test implementation to support it.");
                                break;
                        }
                        break;

                    case nameof(RulesEngine.UpdateRuleAsync):
                        _ = await sut.UpdateRuleAsync(null);
                        break;

                    default:
                        Assert.Fail("Test scenario not supported, please review test implementation to support it.");
                        break;
                }
            });

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType(exceptionType);
            if (actual is ArgumentException argumentException)
            {
                argumentException.Message.Should().Contain(parameterName);
                argumentException.ParamName.Should().Be(parameterName);
            }
        }

        private static Rule CreateTestStubRule()
            => Rule.Create("Test stub")
                .OnRuleset("Test content type")
                .SetContent(new object())
                .Since(DateTime.Parse("2024-08-17", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal))
                .Build()
                .Rule;

        private void SetupMockForConditionsEvalEngine(Func<IConditionNode, IDictionary<string, object>, EvaluationOptions, bool> evalFunc, EvaluationOptions evaluationOptions)
        {
            Mock.Get(this.conditionsEvalEngineMock)
                .Setup(x => x.Eval(
                    It.IsAny<IConditionNode>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.Is<EvaluationOptions>(eo => eo == evaluationOptions)))
                .Returns(evalFunc);
        }

        private void SetupMockForConditionsEvalEngine(bool result, EvaluationOptions evaluationOptions)
        {
            Mock.Get(this.conditionsEvalEngineMock)
                .Setup(x => x.Eval(
                    It.IsAny<IConditionNode>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.Is<EvaluationOptions>(eo => eo == evaluationOptions)))
                .Returns(result);
        }

        private void SetupMockForRulesDataSource(IEnumerable<Rule> rules)
        {
            Mock.Get(this.rulesSourceMock)
                .Setup(x => x.GetRulesAsync(It.IsAny<GetRulesArgs>()))
                .ReturnsAsync(rules);
        }
    }
}