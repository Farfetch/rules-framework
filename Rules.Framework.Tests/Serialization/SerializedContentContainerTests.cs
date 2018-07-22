using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rules.Framework.Serialization;
using Rules.Framework.Tests.TestStubs;

namespace Rules.Framework.Tests.Serialization
{
    [TestClass]
    public class SerializedContentContainerTests
    {
        [TestMethod]
        public void SerializedContentContainer_Init_GivenSerializedContent_DeserializesAndReturnsWhenFetchingContent()
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
            Assert.AreEqual(expected, actual);
        }
    }
}