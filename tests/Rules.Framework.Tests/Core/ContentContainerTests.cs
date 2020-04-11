namespace Rules.Framework.Tests.Core
{
    using System;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Tests.TestStubs;
    using Xunit;

    public class ContentContainerTests
    {
        [Fact]
        public void ContentContainer_ContentType_HavingProvidedContentTypeByCtor_ReturnsProvidedValue()
        {
            // Arrange
            ContentType expected = ContentType.Type1;

            ContentContainer<ContentType> sut = new ContentContainer<ContentType>(expected, (t) => new object());

            // Act
            ContentType actual = sut.ContentType;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void ContentContainer_GetContentAs_HavingProvidedContentByCtorAndGivenCorrectRuntimeType_ReturnsProvidedValue()
        {
            // Arrange
            object expected = "Just some content";

            ContentContainer<ContentType> sut = new ContentContainer<ContentType>(ContentType.Type1, (t) => expected);

            // Act
            object actual = sut.GetContentAs<string>();

            // Assert
            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void ContentContainer_GetContentAs_HavingProvidedContentByCtorAndGivenWrongRuntimeType_ThrowsArgumentException()
        {
            // Arrange
            object expected = "Just some content";

            ContentContainer<ContentType> sut = new ContentContainer<ContentType>(ContentType.Type1, (t) => expected);

            // Act
            ContentTypeException contentTypeException = Assert.Throws<ContentTypeException>(() => sut.GetContentAs<int>());

            // Assert
            contentTypeException.Should().NotBeNull();
            contentTypeException.Message.Should().Be("Cannot cast content to provided type as TContent: System.Int32");
            contentTypeException.InnerException.Should().NotBeNull()
                .And.BeOfType<InvalidCastException>($"{nameof(ContentTypeException)} should happen when an invalid cast is attempted.");
        }
    }
}