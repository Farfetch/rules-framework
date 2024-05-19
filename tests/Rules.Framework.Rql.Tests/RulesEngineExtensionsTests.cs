namespace Rules.Framework.Rql.Tests
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Rql.Tests.Stubs;
    using Xunit;

    public class RulesEngineExtensionsTests
    {
        [Fact]
        public void GetRqlEngine_GivenRulesEngineWithEnumType_BuildsRqlEngineWithDefaultRqlOptions()
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
            var rulesEngine = Mock.Of<IRulesEngine<ContentType, int>>();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => rulesEngine.GetRqlEngine());

            // Assert
            notSupportedException.Message.Should().Be("Rule Query Language is only supported for enum types or strings on TConditionType.");
        }

        [Fact]
        public void GetRqlEngine_GivenRulesEngineWithNonEnumContentType_ThrowsNotSupportedException()
        {
            // Arrange
            var rulesEngine = Mock.Of<IRulesEngine<int, ConditionType>>();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => rulesEngine.GetRqlEngine());

            // Assert
            notSupportedException.Message.Should().Be("Rule Query Language is only supported for enum types or strings on TContentType.");
        }

        [Fact]
        public void GetRqlEngine_GivenRulesEngineWithStringTypes_BuildsRqlEngineWithDefaultRqlOptions()
        {
            // Arrange
            var rulesEngine = Mock.Of<IRulesEngine<string, string>>();

            // Act
            var rqlEngine = rulesEngine.GetRqlEngine();

            // Assert
            rqlEngine.Should().NotBeNull();
        }
    }
}