namespace Rules.Framework.Tests.Extensions
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Extension;
    using Rules.Framework.Generics;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RulesEngineExtensionsTests
    {
        [Fact]
        public void RulesEngineExtensions_ToGenericEngine_Success()
        {
            // Arrange
            var rulesEngine = new RulesEngine<ContentType, ConditionType>(Mock.Of<RulesEngineArgs<ContentType, ConditionType>>());

            //Act
            var genericEngine = rulesEngine.CreateGenericEngine();

            //Arrange
            genericEngine.Should().NotBeNull();
            genericEngine.GetType().Should().Be(typeof(GenericRulesEngine<ContentType, ConditionType>));
        }
    }
}