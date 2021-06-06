namespace Rules.Framework.Tests.Builder
{
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class RuleBuilderExtensionsTests
    {
        [Fact]
        public void WithContent_GivenContentTypeAndContent_BuildsContentContainerWithThemAndSetsOnRuleBuilder()
        {
            // Arrange
            ContentType contentType = ContentType.Type1;
            object content = new object();

            ContentContainer<ContentType> actual = null;
            IRuleBuilder<ContentType, ConditionType> ruleBuilder = Mock.Of<IRuleBuilder<ContentType, ConditionType>>();
            Mock.Get(ruleBuilder).Setup(x => x.WithContentContainer(It.IsAny<ContentContainer<ContentType>>()))
                .Returns(ruleBuilder)
                .Callback<ContentContainer<ContentType>>((cc) => actual = cc);

            // Act
            IRuleBuilder<ContentType, ConditionType> resultRuleBuilder = ruleBuilder.WithContent(contentType, content);

            // Assert
            resultRuleBuilder.Should().NotBeNull().And.BeSameAs(ruleBuilder);
            actual.Should().NotBeNull();
            actual.ContentType.Should().Be(contentType);
            actual.GetContentAs<object>().Should().NotBeNull().And.BeSameAs(content);
        }
    }
}