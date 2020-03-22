namespace Rules.Framework.Providers.MongoDb.Tests.Serialization
{
    using FluentAssertions;
    using Rules.Framework.Providers.MongoDb.Serialization;
    using Rules.Framework.Providers.MongoDb.Tests.TestStubs;
    using Rules.Framework.Serialization;
    using Xunit;

    public class DynamicToStrongTypeContentSerializationProviderTests
    {
        [Fact]
        public void GetContentSerializer_GivenAnyContentTypeValue_ReturnsDynamicToStrongTypeContentSerializer()
        {
            // Arrange
            DynamicToStrongTypeContentSerializationProvider<ContentType> dynamicToStrongTypeContentSerializationProvider = new DynamicToStrongTypeContentSerializationProvider<ContentType>();

            // Act
            IContentSerializer contentSerializer = dynamicToStrongTypeContentSerializationProvider.GetContentSerializer(ContentType.ContentTypeSample);

            // Assert
            contentSerializer.Should().NotBeNull()
                .And.BeOfType<DynamicToStrongTypeContentSerializer>();
        }
    }
}