namespace Rules.Framework.Tests.Builder
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Builder;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class ConfiguredRulesEngineBuilderTests
    {
        [Fact]
        public void Build_WhenCompilationIsEnabled_ReturnsRulesEngineWithCompiledEvaluation()
        {
            // Arrange
            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            var configuredRulesEngineBuilder = new ConfiguredRulesEngineBuilder<ContentType, ConditionType>(rulesDataSource);

            configuredRulesEngineBuilder.Configure(opt =>
            {
                opt.EnableCompilation = true;
            });

            // Act
            var actual = configuredRulesEngineBuilder.Build();

            // Assert
            actual.Should().NotBeNull();
        }

        [Fact]
        public void Build_WhenCompilationIsNotEnabled_ReturnsRulesEngineWithClassicEvaluation()
        {
            // Arrange
            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            var configuredRulesEngineBuilder = new ConfiguredRulesEngineBuilder<ContentType, ConditionType>(rulesDataSource);

            // Act
            var actual = configuredRulesEngineBuilder.Build();

            // Assert
            actual.Should().NotBeNull();
        }

        [Fact]
        public void Configure_GivenOptionsConfigurationAction_SetsOptionsAndValidates()
        {
            // Arrange
            var rulesDataSource = Mock.Of<IRulesDataSource<ContentType, ConditionType>>();
            var configuredRulesEngineBuilder = new ConfiguredRulesEngineBuilder<ContentType, ConditionType>(rulesDataSource);

            // Act
            var actual = configuredRulesEngineBuilder.Configure(opt =>
            {
                opt.MissingConditionBehavior = MissingConditionBehaviors.Discard;
            });

            // Assert
            actual.Should().NotBeNull()
                .And.BeSameAs(configuredRulesEngineBuilder);
        }
    }
}