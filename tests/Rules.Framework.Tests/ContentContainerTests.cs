namespace Rules.Framework.Tests
{
    using System;
    using FluentAssertions;
    using Rules.Framework;
    using Xunit;

    public class ContentContainerTests
    {
        [Fact]
        public void ContentContainer_GetContentAs_HavingProvidedContentByCtorAndGivenCorrectRuntimeType_ReturnsProvidedValue()
        {
            // Arrange
            object expected = "Just some content";

            var sut = new ContentContainer(_ => expected);

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

            var sut = new ContentContainer(_ => expected);

            // Act
            var contentTypeException = Assert.Throws<ContentTypeException>(() => sut.GetContentAs<int>());

            // Assert
            contentTypeException.Should().NotBeNull();
            contentTypeException.Message.Should().Be("Cannot cast content to provided type as TContent: System.Int32");
            contentTypeException.InnerException.Should().NotBeNull()
                .And.BeOfType<InvalidCastException>($"{nameof(ContentTypeException)} should happen when an invalid cast is attempted.");
        }
    }
}