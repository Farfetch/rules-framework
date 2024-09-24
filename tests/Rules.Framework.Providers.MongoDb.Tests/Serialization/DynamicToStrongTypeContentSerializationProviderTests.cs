namespace Rules.Framework.Providers.MongoDb.Tests.Serialization
{
    using FluentAssertions;
    using Rules.Framework.Providers.MongoDb.Serialization;
    using Rules.Framework.Providers.MongoDb.Tests.TestStubs;
    using Xunit;

    public class DynamicToStrongTypeContentSerializationProviderTests
    {
        [Fact]
        public void GetContentSerializer_GivenAnyContentTypeValue_ReturnsDynamicToStrongTypeContentSerializer()
        {
            // Arrange
            var dynamicToStrongTypeContentSerializationProvider = new DynamicToStrongTypeContentSerializationProvider();

            // Act
            var contentSerializer = dynamicToStrongTypeContentSerializationProvider.GetContentSerializer(ContentType.ContentTypeSample.ToString());

            // Assert
            contentSerializer.Should().NotBeNull()
                .And.BeOfType<DynamicToStrongTypeContentSerializer>();
        }
    }
}