namespace Rules.Framework.Tests.Serialization
{
    using System;
    using FluentAssertions;
    using Moq;
    using Rules.Framework.Serialization;
    using Rules.Framework.Tests.Stubs;
    using Xunit;

    public class SerializedContentContainerTests
    {
        [Fact]
        public void Init_GivenSerializedContent_DeserializesAndReturnsWhenFetchingContent()
        {
            // Arrange
            var expectedContentType = ContentType.Type1.ToString();
            var serializedContent = new object();
            object expected = 19m;

            var mockContentSerializer = new Mock<IContentSerializer>();
            mockContentSerializer.Setup(x => x.Deserialize(It.IsAny<object>(), It.IsAny<Type>()))
                .Returns(expected);

            var mockContentSerializationProvider = new Mock<IContentSerializationProvider>();
            mockContentSerializationProvider.Setup(x => x.GetContentSerializer(It.Is<string>(y => y == expectedContentType)))
                .Returns(mockContentSerializer.Object);

            var sut = new SerializedContentContainer(expectedContentType, serializedContent, mockContentSerializationProvider.Object);

            // Act
            var actual = sut.GetContentAs<decimal>();

            // Assert
            actual.Should().Be(expected.As<decimal>());
        }
    }
}