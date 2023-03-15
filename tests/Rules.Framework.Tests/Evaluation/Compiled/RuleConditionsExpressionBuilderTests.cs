namespace Rules.Framework.Tests.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using DiffPlex.DiffBuilder;
    using ExpressionDebugger;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.Compiled;
    using Rules.Framework.Evaluation.Compiled.ExpressionBuilders;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RuleConditionsExpressionBuilderTests
    {
        internal static IEnumerable<(string, EvaluationContext<ConditionType>, bool)> AndComposedConditionNodeScenarios => new[]
        {
            (
                "Scenario 1 - MissingConditionsBehavior = 'Discard', MatchMode = 'Exact', and only contains condition for 'NumberOfSales'",
                new EvaluationContext<ConditionType>(
                    new Dictionary<ConditionType, object>
                    {
                        { ConditionType.NumberOfSales, 500 },
                    },
                    MatchModes.Exact,
                    MissingConditionBehaviors.Discard),
                false
            ),
            (
                "Scenario 2 - MissingConditionsBehavior = 'Discard', MatchMode = 'Exact', and both needed conditions",
                new EvaluationContext<ConditionType>(
                    new Dictionary<ConditionType, object>
                    {
                        { ConditionType.NumberOfSales, 500 },
                        { ConditionType.IsoCountryCode, "PT" },
                    },
                    MatchModes.Exact,
                    MissingConditionBehaviors.Discard),
                true
            ),
            (
                "Scenario 3 - MissingConditionsBehavior = 'UseDataTypeDefault', MatchMode = 'Exact', and both needed conditions",
                new EvaluationContext<ConditionType>(
                    new Dictionary<ConditionType, object>
                    {
                        { ConditionType.NumberOfSales, 500 },
                        { ConditionType.IsoCountryCode, "PT" },
                    },
                    MatchModes.Exact,
                    MissingConditionBehaviors.UseDataTypeDefault),
                true
            ),
            (
                "Scenario 4 - MissingConditionsBehavior = 'UseDataTypeDefault', MatchMode = 'Search', and both needed conditions",
                new EvaluationContext<ConditionType>(
                    new Dictionary<ConditionType, object>
                    {
                        { ConditionType.NumberOfSales, 500 },
                        { ConditionType.IsoCountryCode, "PT" },
                    },
                    MatchModes.Search,
                    MissingConditionBehaviors.UseDataTypeDefault),
                true
            ),
            (
                "Scenario 5 - MissingConditionsBehavior = 'UseDataTypeDefault', MatchMode = 'Search', and only contains condition for 'NumberOfSales'",
                new EvaluationContext<ConditionType>(
                    new Dictionary<ConditionType, object>
                    {
                        { ConditionType.NumberOfSales, 500 },
                    },
                    MatchModes.Search,
                    MissingConditionBehaviors.UseDataTypeDefault),
                true
            )
        };

        internal static IEnumerable<(string, EvaluationContext<ConditionType>, bool)> OrComposedConditionNodeScenarios => new[]
        {
            (
                "Scenario 1 - MissingConditionsBehavior = 'Discard', MatchMode = 'Exact', and only contains condition for 'NumberOfSales'",
                new EvaluationContext<ConditionType>(
                    new Dictionary<ConditionType, object>
                    {
                        { ConditionType.NumberOfSales, 500 },
                    },
                    MatchModes.Exact,
                    MissingConditionBehaviors.Discard),
                true
            ),
            (
                "Scenario 2 - MissingConditionsBehavior = 'Discard', MatchMode = 'Exact', and both needed conditions",
                new EvaluationContext<ConditionType>(
                    new Dictionary<ConditionType, object>
                    {
                        { ConditionType.NumberOfSales, 500 },
                        { ConditionType.IsoCountryCode, "PT" },
                    },
                    MatchModes.Exact,
                    MissingConditionBehaviors.Discard),
                true
            ),
            (
                "Scenario 3 - MissingConditionsBehavior = 'UseDataTypeDefault', MatchMode = 'Exact', and both needed conditions",
                new EvaluationContext<ConditionType>(
                    new Dictionary<ConditionType, object>
                    {
                        { ConditionType.NumberOfSales, 500 },
                        { ConditionType.IsoCountryCode, "PT" },
                    },
                    MatchModes.Exact,
                    MissingConditionBehaviors.UseDataTypeDefault),
                true
            ),
            (
                "Scenario 4 - MissingConditionsBehavior = 'UseDataTypeDefault', MatchMode = 'Search', and both needed conditions",
                new EvaluationContext<ConditionType>(
                    new Dictionary<ConditionType, object>
                    {
                        { ConditionType.NumberOfSales, 500 },
                        { ConditionType.IsoCountryCode, "PT" },
                    },
                    MatchModes.Search,
                    MissingConditionBehaviors.UseDataTypeDefault),
                true
            ),
            (
                "Scenario 5 - MissingConditionsBehavior = 'UseDataTypeDefault', MatchMode = 'Search', and only contains condition for 'NumberOfSales'",
                new EvaluationContext<ConditionType>(
                    new Dictionary<ConditionType, object>
                    {
                        { ConditionType.NumberOfSales, 500 },
                    },
                    MatchModes.Search,
                    MissingConditionBehaviors.UseDataTypeDefault),
                true
            )
        };

        [Fact]
        public void BuildExpression_GivenAndComposedConditionNodeWith2ChildValueConditionNodes_BuildsLambdaExpression()
        {
            // Arrange
            string expectedScript;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Rules.Framework.Tests.Evaluation.Compiled.RuleConditionsExpressionBuilderTests.GoldenFile1.csx"))
            using (var streamReader = new StreamReader(stream))
            {
                expectedScript = streamReader.ReadToEnd();
            }
            var valueConditionNode1
                = new ValueConditionNode<ConditionType>(DataTypes.Integer, ConditionType.NumberOfSales, Operators.Equal, 100);
            var valueConditionNode2
                = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "GB");

            var composedConditionNode
                = new ComposedConditionNode<ConditionType>(LogicalOperators.And, new[] { valueConditionNode1, valueConditionNode2 });

            var valueConditionNodeExpressionBuilder = Mock.Of<IValueConditionNodeExpressionBuilder>();
            Mock.Get(valueConditionNodeExpressionBuilder)
                .Setup(x => x.Build(It.IsAny<IExpressionBlockBuilder>(), It.IsAny<BuildValueConditionNodeExpressionArgs>()))
                .Callback<IExpressionBlockBuilder, BuildValueConditionNodeExpressionArgs>(
                (builder, args) =>
                {
                    builder.Assign(args.ResultVariableExpression, builder.Constant(true));
                    builder.AddExpression(builder.Empty());
                });

            var valueConditionNodeExpressionBuilderProvider = Mock.Of<IValueConditionNodeExpressionBuilderProvider>();
            Mock.Get(valueConditionNodeExpressionBuilderProvider)
                .Setup(x => x.GetExpressionBuilder(It.Is(Multiplicities.OneToOne, StringComparer.Ordinal)))
                .Returns(valueConditionNodeExpressionBuilder);

            var dataTypeConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypeConfigurationProvider)
                .Setup(x => x.GetDataTypeConfiguration(DataTypes.String))
                .Returns(DataTypeConfiguration.Create(DataTypes.String, typeof(string), null));
            Mock.Get(dataTypeConfigurationProvider)
                .Setup(x => x.GetDataTypeConfiguration(DataTypes.Integer))
                .Returns(DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), 0));

            var conditionsTreeCompiler = new RuleConditionsExpressionBuilder<ConditionType>(
                valueConditionNodeExpressionBuilderProvider,
                dataTypeConfigurationProvider);

            // Act
            var expression = conditionsTreeCompiler.BuildExpression(composedConditionNode);

            // Assert
            expression.Should().NotBeNull();
            var actualScript = expression.ToScript();
            var diffResult = SideBySideDiffBuilder.Diff(expectedScript, actualScript, ignoreWhiteSpace: true);
            diffResult.NewText.HasDifferences.Should().BeFalse();

            Func<EvaluationContext<ConditionType>, bool> compiledLambdaExpression = null;
            FluentActions.Invoking(() => compiledLambdaExpression = expression.Compile())
                .Should()
                .NotThrow("expression should be compilable");

            foreach (var scenario in AndComposedConditionNodeScenarios)
            {
                bool? result = null;
                FluentActions.Invoking(() => result = compiledLambdaExpression.Invoke(scenario.Item2))
                    .Should()
                    .NotThrow($"compiled expression should be executable under scenario: {scenario.Item1}");

                result.Should().Be(scenario.Item3);
            }
        }

        [Fact]
        public void BuildExpression_GivenOrComposedConditionNodeWith2ChildValueConditionNodes_BuildsLambdaExpression()
        {
            // Arrange
            string expectedScript;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Rules.Framework.Tests.Evaluation.Compiled.RuleConditionsExpressionBuilderTests.GoldenFile2.csx"))
            using (var streamReader = new StreamReader(stream))
            {
                expectedScript = streamReader.ReadToEnd();
            }
            var valueConditionNode1
                = new ValueConditionNode<ConditionType>(DataTypes.Integer, ConditionType.NumberOfSales, Operators.Equal, 100);
            var valueConditionNode2
                = new ValueConditionNode<ConditionType>(DataTypes.String, ConditionType.IsoCountryCode, Operators.Equal, "GB");

            var composedConditionNode
                = new ComposedConditionNode<ConditionType>(LogicalOperators.Or, new[] { valueConditionNode1, valueConditionNode2 });

            var valueConditionNodeExpressionBuilder = Mock.Of<IValueConditionNodeExpressionBuilder>();
            Mock.Get(valueConditionNodeExpressionBuilder)
                .Setup(x => x.Build(It.IsAny<IExpressionBlockBuilder>(), It.IsAny<BuildValueConditionNodeExpressionArgs>()))
                .Callback<IExpressionBlockBuilder, BuildValueConditionNodeExpressionArgs>(
                (builder, args) =>
                {
                    builder.Assign(args.ResultVariableExpression, builder.Constant(true));
                    builder.AddExpression(builder.Empty());
                });

            var valueConditionNodeExpressionBuilderProvider = Mock.Of<IValueConditionNodeExpressionBuilderProvider>();
            Mock.Get(valueConditionNodeExpressionBuilderProvider)
                .Setup(x => x.GetExpressionBuilder(It.Is(Multiplicities.OneToOne, StringComparer.Ordinal)))
                .Returns(valueConditionNodeExpressionBuilder);

            var dataTypeConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypeConfigurationProvider)
                .Setup(x => x.GetDataTypeConfiguration(DataTypes.String))
                .Returns(DataTypeConfiguration.Create(DataTypes.String, typeof(string), null));
            Mock.Get(dataTypeConfigurationProvider)
                .Setup(x => x.GetDataTypeConfiguration(DataTypes.Integer))
                .Returns(DataTypeConfiguration.Create(DataTypes.Integer, typeof(int), 0));

            var conditionsTreeCompiler = new RuleConditionsExpressionBuilder<ConditionType>(
                valueConditionNodeExpressionBuilderProvider,
                dataTypeConfigurationProvider);

            // Act
            var expression = conditionsTreeCompiler.BuildExpression(composedConditionNode);

            // Assert
            expression.Should().NotBeNull();
            var actualScript = expression.ToScript();
            var diffResult = SideBySideDiffBuilder.Diff(expectedScript, actualScript, ignoreWhiteSpace: true);
            diffResult.NewText.HasDifferences.Should().BeFalse();

            Func<EvaluationContext<ConditionType>, bool> compiledLambdaExpression = null;
            FluentActions.Invoking(() => compiledLambdaExpression = expression.Compile())
                .Should()
                .NotThrow("expression should be compilable");

            foreach (var scenario in OrComposedConditionNodeScenarios)
            {
                bool? result = null;
                FluentActions.Invoking(() => result = compiledLambdaExpression.Invoke(scenario.Item2))
                    .Should()
                    .NotThrow($"compiled expression should be executable under scenario: {scenario.Item1}");

                result.Should().Be(scenario.Item3);
            }
        }

        [Fact]
        public void BuildExpression_GivenUnknownConditionNode_ThrowsNotSupportedException()
        {
            // Arrange
            var stubConditionNode = new StubConditionNode<ConditionType>();

            var valueConditionNodeExpressionBuilderProvider = Mock.Of<IValueConditionNodeExpressionBuilderProvider>();
            var dataTypeConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();

            var ruleConditionsExpressionBuilder = new RuleConditionsExpressionBuilder<ConditionType>(
                valueConditionNodeExpressionBuilderProvider,
                dataTypeConfigurationProvider);

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => ruleConditionsExpressionBuilder.BuildExpression(stubConditionNode));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(nameof(StubConditionNode<ConditionType>));
        }

        [Fact]
        public void BuildExpression_GivenUnsupportedLogicalOperatorForComposedConditionNode_ThrowsNotSupportedException()
        {
            // Arrange
            var composedConditionNode = new ComposedConditionNode<ConditionType>(LogicalOperators.Eval, Enumerable.Empty<IConditionNode<ConditionType>>());

            var valueConditionNodeExpressionBuilderProvider = Mock.Of<IValueConditionNodeExpressionBuilderProvider>();
            var dataTypeConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();

            var ruleConditionsExpressionBuilder = new RuleConditionsExpressionBuilder<ConditionType>(
                valueConditionNodeExpressionBuilderProvider,
                dataTypeConfigurationProvider);

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => ruleConditionsExpressionBuilder.BuildExpression(composedConditionNode));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(nameof(LogicalOperators.Eval));
        }
    }
}