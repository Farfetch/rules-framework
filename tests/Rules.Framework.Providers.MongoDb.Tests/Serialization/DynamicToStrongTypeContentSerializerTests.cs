namespace Rules.Framework.Providers.MongoDb.Tests.Serialization
{
    using System;
    using System.Dynamic;
    using System.Runtime.Serialization;
    using FluentAssertions;
    using Rules.Framework.Providers.MongoDb.Serialization;
    using Rules.Framework.Providers.MongoDb.Tests.TestStubs;
    using Xunit;

    public class DynamicToStrongTypeContentSerializerTests
    {
        [Fact]
        public void Deserialize_GivenNullSerializedContent_ThrowsArgumentNullException()
        {
            // Arrange
            object serializedContent = null;
            Type type = null;

            DynamicToStrongTypeContentSerializer dynamicToStrongTypeContentSerializer = new DynamicToStrongTypeContentSerializer();

            // Act
            ArgumentNullException argumentNullException = Assert.Throws<ArgumentNullException>(() =>
            {
                dynamicToStrongTypeContentSerializer.Deserialize(serializedContent, type);
            });

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(serializedContent));
        }

        [Fact]
        public void Deserialize_GivenNullType_ThrowsArgumentNullException()
        {
            // Arrange
            object serializedContent = new object();
            Type type = null;

            DynamicToStrongTypeContentSerializer dynamicToStrongTypeContentSerializer = new DynamicToStrongTypeContentSerializer();

            // Act
            ArgumentNullException argumentNullException = Assert.Throws<ArgumentNullException>(() =>
            {
                dynamicToStrongTypeContentSerializer.Deserialize(serializedContent, type);
            });

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(type));
        }

        [Fact]
        public void Deserialize_GivenNonDynamicSerializedContent_ThrowsNotSupportedException()
        {
            // Arrange
            object serializedContent = new object();
            Type type = typeof(ContentStub);

            DynamicToStrongTypeContentSerializer dynamicToStrongTypeContentSerializer = new DynamicToStrongTypeContentSerializer();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() =>
            {
                dynamicToStrongTypeContentSerializer.Deserialize(serializedContent, type);
            });

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be($"The serialized content type is not supported for deserialization: {typeof(object).FullName}");
        }

        [Fact]
        public void Deserialize_GivenTypeNoDefaultCtor_ThrowsNotSupportedException()
        {
            // Arrange
            dynamic serializedContent = new ExpandoObject();
            Type type = typeof(MissingDefaultCtorContentStub);

            DynamicToStrongTypeContentSerializer dynamicToStrongTypeContentSerializer = new DynamicToStrongTypeContentSerializer();

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() =>
            {
                dynamicToStrongTypeContentSerializer.Deserialize(serializedContent, type);
            });

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be($"The target type '{typeof(MissingDefaultCtorContentStub).FullName}' must define a default (no parameters) constructor.");
        }

        [Fact]
        public void Deserialize_GivenSerializedContentWithPropertyMissingOnType_ThrowsSerializationException()
        {
            // Arrange
            dynamic serializedContent = new ExpandoObject();
            serializedContent.Prop1 = 1;
            serializedContent.Prop2 = true;
            Type type = typeof(MissingPropertyContentStub);

            DynamicToStrongTypeContentSerializer dynamicToStrongTypeContentSerializer = new DynamicToStrongTypeContentSerializer();

            // Act
            SerializationException serializationException = Assert.Throws<SerializationException>(() =>
            {
                dynamicToStrongTypeContentSerializer.Deserialize(serializedContent, type);
            });

            // Assert
            serializationException.Should().NotBeNull();
            serializationException.Message.Should().Be($"Property 'Prop2' does not have a matching property by the same name on type '{type.FullName}'.");
        }

        [Fact]
        public void Deserialize_GivenSerializedContentWithPropertyValueTypeDifferentFromType_ThrowsSerializationException()
        {
            // Arrange
            dynamic serializedContent = new ExpandoObject();
            serializedContent.Prop01 = 1;
            serializedContent.Prop03 = "WRONG VALUE";
            Type type = typeof(ContentStub);

            DynamicToStrongTypeContentSerializer dynamicToStrongTypeContentSerializer = new DynamicToStrongTypeContentSerializer();

            // Act
            SerializationException serializationException = Assert.Throws<SerializationException>(() =>
            {
                dynamicToStrongTypeContentSerializer.Deserialize(serializedContent, type);
            });

            // Assert
            serializationException.Should().NotBeNull();
            serializationException.Message.Should().Be($"An invalid value has been provided for property 'Prop03' of type '{type.FullName}': 'WRONG VALUE'.");
            serializationException.InnerException.Should().BeOfType<FormatException>();
        }

        [Fact]
        public void Deserialize_GivenCorrectSerializedContentAndType_ReturnsDeserializedTypeInstance()
        {
            // Arrange
            dynamic serializedContent = new ExpandoObject();

            // Remember that InvariantCulture formats is an assumption!
            serializedContent.Prop01 = 1;
            serializedContent.Prop02 = "TEST";
            serializedContent.Prop03 = 30.3m;
            serializedContent.Prop04 = "e986380c-ca88-47dd-b417-15f8beb26d9c";
            serializedContent.Prop05 = "ContentTypeSample";
            serializedContent.Prop06 = "123";
            serializedContent.Prop07 = "95.78";
            serializedContent.Prop08 = true;
            serializedContent.Prop09 = "false";
            serializedContent.Prop10 = DateTime.Parse("2020-03-21Z");
            serializedContent.Prop11 = "2020-03-21 15:26:58Z";
            Type type = typeof(ContentStub);

            DynamicToStrongTypeContentSerializer dynamicToStrongTypeContentSerializer = new DynamicToStrongTypeContentSerializer();

            // Act
            object deserializedContent = dynamicToStrongTypeContentSerializer.Deserialize(serializedContent, type);

            // Assert
            deserializedContent.Should().BeOfType(type);

            ContentStub contentStub = deserializedContent.As<ContentStub>();
            contentStub.Prop01.Should().Be(1);
            contentStub.Prop02.Should().Be("TEST");
            contentStub.Prop03.Should().Be(30.3m);
            contentStub.Prop04.Should().Be(Guid.Parse("e986380c-ca88-47dd-b417-15f8beb26d9c"));
            contentStub.Prop05.Should().Be(ContentType.ContentTypeSample);
            contentStub.Prop06.Should().Be(123);
            contentStub.Prop07.Should().Be(95.78m);
            contentStub.Prop08.Should().BeTrue();
            contentStub.Prop09.Should().BeFalse();
            contentStub.Prop10.Should().Be(DateTime.Parse("2020-03-21Z"));
            contentStub.Prop11.Should().Be(DateTime.Parse("2020-03-21 15:26:58Z"));
        }
    }
}