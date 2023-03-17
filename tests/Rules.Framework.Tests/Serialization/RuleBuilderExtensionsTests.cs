namespace Rules.Framework.Tests.Serialization
{
    using System;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Serialization;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class RuleBuilderExtensionsTests
    {
        [Fact]
        public void WithSerializedContent_GivenNullRuleBuilder_ThrowsArgumentNullException()
        {
            // Assert
            IRuleBuilder<ContentType, ConditionType> ruleBuilder = null;
            IContentSerializationProvider<ContentType> contentSerializationProvider = null;

            // Act
            ArgumentNullException argumentNullException = Assert
                .Throws<ArgumentNullException>(() => ruleBuilder.WithSerializedContent(ContentType.Type1, "TEST", contentSerializationProvider));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(ruleBuilder));
        }

        [Fact]
        public void WithSerializedContent_GivenNullContentSerializationProvider_ThrowsArgumentNullException()
        {
            // Assert
            IRuleBuilder<ContentType, ConditionType> ruleBuilder = Mock.Of<IRuleBuilder<ContentType, ConditionType>>();
            IContentSerializationProvider<ContentType> contentSerializationProvider = null;

            // Act
            ArgumentNullException argumentNullException = Assert
                .Throws<ArgumentNullException>(() => ruleBuilder.WithSerializedContent(ContentType.Type1, "TEST", contentSerializationProvider));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(contentSerializationProvider));
        }

        [Fact]
        public void WithSerializerContent_GivenRuleBuilderAndContentSerializationProvider_SetsContentContainerAsSerializedContentContainer()
        {
            // Assert
            ContentContainer<ContentType> contentContainer = null;
            ContentType contentType = ContentType.Type1;
            string content = "TEST";

            IRuleBuilder<ContentType, ConditionType> ruleBuilder = Mock.Of<IRuleBuilder<ContentType, ConditionType>>();
            Mock.Get(ruleBuilder)
                .Setup(x => x.WithContentContainer(It.IsAny<ContentContainer<ContentType>>()))
                .Callback<ContentContainer<ContentType>>((x) => contentContainer = x)
                .Returns(ruleBuilder);

            IContentSerializer contentSerializer = Mock.Of<IContentSerializer>();
            Mock.Get(contentSerializer)
                .Setup(x => x.Deserialize(It.IsAny<object>(), It.IsAny<Type>()))
                .Returns(content);

            IContentSerializationProvider<ContentType> contentSerializationProvider = Mock.Of<IContentSerializationProvider<ContentType>>();
            Mock.Get(contentSerializationProvider)
                .Setup(x => x.GetContentSerializer(contentType))
                .Returns(contentSerializer);

            // Act
            IRuleBuilder<ContentType, ConditionType> returnRuleBuilder = ruleBuilder.WithSerializedContent(contentType, content, contentSerializationProvider);

            // Assert
            returnRuleBuilder.Should().BeSameAs(ruleBuilder);
            contentContainer.Should().NotBeNull()
                .And.BeOfType<SerializedContentContainer<ContentType>>();
        }
    }
}