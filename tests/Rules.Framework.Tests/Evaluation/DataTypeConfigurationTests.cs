namespace Rules.Framework.Tests.Evaluation
{
    using System;
    using FluentAssertions;
    using Rules.Framework;
    using Rules.Framework.Evaluation;
    using Xunit;

    public class DataTypeConfigurationTests
    {
        [Fact]
        public void Create_GivenDataTypeWithNonNullSystemTypeAndADefault_ReturnsNewInstance()
        {
            // Arrange
            var dataType = DataTypes.Integer;
            var type = typeof(int);
            var @default = 0;

            // Act
            var dataTypeConfiguration = DataTypeConfiguration.Create(dataType, type, @default);

            // Assert
            dataTypeConfiguration.Should().NotBeNull();
            dataTypeConfiguration.Default.Should().Be(@default);
            dataTypeConfiguration.DataType.Should().Be(dataType);
            dataTypeConfiguration.Type.Should().Be(type);
        }

        [Fact]
        public void Create_GivenDataTypeWithNullSystemTypeAndADefault_ThrowsArgumentNullExcetion()
        {
            // Assert
            var dataType = DataTypes.Integer;
            var @default = 0;

            // Act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => DataTypeConfiguration.Create(dataType, null, @default));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be("type");
        }
    }
}