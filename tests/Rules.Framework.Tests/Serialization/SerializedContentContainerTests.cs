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
            ContentType expectedContentType = ContentType.Type1;
            object serializedContent = new object();
            object expected = 19m;

            Mock<IContentSerializer> mockContentSerializer = new Mock<IContentSerializer>();
            mockContentSerializer.Setup(x => x.Deserialize(It.IsAny<object>(), It.IsAny<Type>()))
                .Returns(expected);

            Mock<IContentSerializationProvider<ContentType>> mockContentSerializationProvider = new Mock<IContentSerializationProvider<ContentType>>();
            mockContentSerializationProvider.Setup(x => x.GetContentSerializer(It.Is<ContentType>(y => y == expectedContentType)))
                .Returns(mockContentSerializer.Object);

            SerializedContentContainer<ContentType> sut = new SerializedContentContainer<ContentType>(expectedContentType, serializedContent, mockContentSerializationProvider.Object);

            // Act
            decimal actual = sut.GetContentAs<decimal>();

            // Assert
            actual.Should().Be(expected.As<decimal>());
        }
    }
}