namespace Rules.Framework.Rql.Tests
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql.Tests.Stubs;
    using Xunit;

    public class RqlEngineBuilderTests
    {
        [Fact]
        public void Build_GivenNullRqlOptions_ThrowsArgumentNullException()
        {
            // Arrange
            var rulesEngine = Mock.Of<IRulesEngine<ContentType, ConditionType>>();

            // Act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() =>
                RqlEngineBuilder<ContentType, ConditionType>.CreateRqlEngine(rulesEngine)
                    .WithOptions(null));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be("options");
        }

        [Fact]
        public void Build_GivenNullRulesEngine_ThrowsArgumentNullException()
        {
            // Act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() =>
                RqlEngineBuilder<ContentType, ConditionType>.CreateRqlEngine(null));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be("rulesEngine");
        }

        [Fact]
        public void Build_GivenRulesEngineAndRqlOptions_BuildsRqlEngine()
        {
            // Arrange
            var rulesEngine = Mock.Of<IRulesEngine<ContentType, ConditionType>>();
            var rqlOptions = RqlOptions.NewWithDefaults();

            // Act
            var rqlEngine = RqlEngineBuilder<ContentType, ConditionType>.CreateRqlEngine(rulesEngine)
                .WithOptions(rqlOptions)
                .Build();

            // Assert
            rqlEngine.Should().NotBeNull();
        }
    }
}