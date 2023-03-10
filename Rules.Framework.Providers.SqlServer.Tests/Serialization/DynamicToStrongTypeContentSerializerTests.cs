
namespace Rules.Framework.Providers.SqlServer.Tests.Serialization
{
    using System.Diagnostics.CodeAnalysis;
    using System.Dynamic;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Rules.Framework.Providers.SqlServer.DataModel;
    using Rules.Framework.Providers.SqlServer.Serialization;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class DynamicToStrongTypeContentSerializerTests
    {
        [Fact]
        public void DynamicToStrongTypeContentSerializer_DeserializeString_ReturnCorrectResult()
        {
            // Arrange
            var objectToDeserialize = "TestObject";

            var dynamicToStrongTypeContentSerializer = new DynamicToStrongTypeContentSerializer();

            // Act
            var result = dynamicToStrongTypeContentSerializer.Deserialize(objectToDeserialize, objectToDeserialize.GetType());

            // Assert
            result.Should().Be(objectToDeserialize);


        }

        [Theory]
        [InlineData("TestObject", null)]
        [InlineData(null, typeof(string))]
        public void DynamicToStrongTypeContentSerializer_IncorrectParameters_ReturnException(object objectToDeserialize, Type type)
        {
            // Arrange
            var dynamicToStrongTypeContentSerializer = new DynamicToStrongTypeContentSerializer();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => dynamicToStrongTypeContentSerializer.Deserialize(objectToDeserialize, type));

        }

        [Fact]
        public void DynamicToStrongTypeContentSerializer_NotSupportedObject_ReturnsExceptions()
        {
            // Arrange
            var objectToDeserialize = new Dictionary<string, object>()
            {
                { "TestKey", "TestObject" }
            };

            var dynamicToStrongTypeContentSerializer = new DynamicToStrongTypeContentSerializer();

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => dynamicToStrongTypeContentSerializer.Deserialize(objectToDeserialize, objectToDeserialize.GetType()));


        }


        [Fact]
        public void DynamicToStrongTypeContentSerializer_DeserializeDictionaryIncorrectly_ThrowSerializationException()
        {
            // Arrange
            var objectToDeserialize = new ExpandoObject() as IDictionary<string, object>;
            objectToDeserialize.Add("NewProp", "TestObject");

            var dynamicToStrongTypeContentSerializer = new DynamicToStrongTypeContentSerializer();

            // Act & Assert
            Assert.Throws<SerializationException>(() => dynamicToStrongTypeContentSerializer.Deserialize(objectToDeserialize, objectToDeserialize.GetType()));


        }

        [Fact]
        public void DynamicToStrongTypeContentSerializer_DeserializeDictionary_ReturnCorrectResult()
        {
            // Arrange
            var objectToDeserialize = new ExpandoObject() as IDictionary<string, object>;
            objectToDeserialize.Add("Name", "RuleDataModel");

            var type = typeof(RuleDataModel);

            var expectedResult = new RuleDataModel()
            {
                Name = "RuleDataModel"
            };

            var dynamicToStrongTypeContentSerializer = new DynamicToStrongTypeContentSerializer();

            // Act
            var result = dynamicToStrongTypeContentSerializer.Deserialize(objectToDeserialize, type);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);


        }
    }
}
