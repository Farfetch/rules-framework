namespace Rules.Framework.Tests.Core
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rules.Framework.Core;
    using Rules.Framework.Tests.TestStubs;

    [TestClass]
    public class ContentContainerTests
    {
        [TestMethod]
        public void ContentContainer_ContentType_HavingProvidedContentTypeByCtor_ReturnsProvidedValue()
        {
            // Arrange
            ContentType expected = ContentType.Type1;

            ContentContainer<ContentType> sut = new ContentContainer<ContentType>(expected, (t) => new object());

            // Act
            ContentType actual = sut.ContentType;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ContentContainer_GetContentAs_HavingProvidedContentByCtorAndGivenCorrectRuntimeType_ReturnsProvidedValue()
        {
            // Arrange
            object expected = "Just some content";

            ContentContainer<ContentType> sut = new ContentContainer<ContentType>(ContentType.Type1, (t) => expected);

            // Act
            object actual = sut.GetContentAs<string>();

            // Assert
            Assert.AreSame(expected, actual);
        }

        [TestMethod]
        public void ContentContainer_GetContentAs_HavingProvidedContentByCtorAndGivenWrongRuntimeType_ThrowsArgumentException()
        {
            // Arrange
            object expected = "Just some content";

            ContentContainer<ContentType> sut = new ContentContainer<ContentType>(ContentType.Type1, (t) => expected);

            // Assert
            Assert.ThrowsException<ContentTypeException>(() =>
            {
                // Act
                sut.GetContentAs<int>();
            });
        }
    }
}