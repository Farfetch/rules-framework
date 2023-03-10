namespace Rules.Framework.Tests.Extensions
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Extension;
    using Rules.Framework.Generics;
    using Rules.Framework.Source;
    using Rules.Framework.Tests.TestStubs;
    using Rules.Framework.Validation;
    using Xunit;

    public class RulesEngineExtensionsTests
    {
        [Fact]
        public void RulesEngineExtensions_ToGenericEngine_Success()
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
            var genericEngine = rulesEngine.CreateGenericEngine();

            //Arrange
            genericEngine.Should().NotBeNull();
            genericEngine.GetType().Should().Be(typeof(GenericRulesEngine<ContentType, ConditionType>));
        }
    }
}