namespace Rules.Framework.Rql.Tests
{
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class RulesEngineExtensionsTests
    {
        [Fact]
        public void GetRqlEngine_GivenRulesEngine_BuildsRqlEngineWithDefaultRqlOptions()
        {
            // Arrange
            var rulesEngine = Mock.Of<IRulesEngine>();

            // Act
            var rqlEngine = rulesEngine.GetRqlEngine();

            // Assert
            rqlEngine.Should().NotBeNull();
        }
    }
}