namespace Rules.Framework.Rql.Tests
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql.Tests.Stubs;
    using Xunit;

    public class RulesEngineExtensionsTests
    {
        [Fact]
        public void GetRqlEngine_GivenRulesEngine_BuildsRqlEngineWithDefaultRqlOptions()
        {
            // Arrange
            var rulesEngine = Mock.Of<IRulesEngine<ContentType, ConditionType>>();

            // Act
            var rqlEngine = rulesEngine.GetRqlEngine();

            // Assert
            rqlEngine.Should().NotBeNull();
        }

        [Fact]
        public void GetRqlEngine_GivenRulesEngineWithNonEnumConditionType_ThrowsNotSupportedException()
        {
            // Arrange
            var rulesEngine = Mock.Of<IRulesEngine<ContentType, string>>();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => rulesEngine.GetRqlEngine());

            // Assert
            notSupportedException.Message.Should().Be("Rule Query Language is not supported for non-enum types of TConditionType.");
        }

        [Fact]
        public void GetRqlEngine_GivenRulesEngineWithNonEnumContentType_ThrowsNotSupportedException()
        {
            // Arrange
            var rulesEngine = Mock.Of<IRulesEngine<string, ConditionType>>();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => rulesEngine.GetRqlEngine());

            // Assert
            notSupportedException.Message.Should().Be("Rule Query Language is not supported for non-enum types of TContentType.");
        }
    }
}