namespace Rules.Framework.Tests.Evaluation
{
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class DataTypeConfigurationTests
    {
        [Fact]
        public void Create_GivenDataTypeWithNonNullSystemTypeAndADefault_ReturnsNewInstance()
        {
            // Arrange
            DataTypes dataType = DataTypes.Integer;
            Type type = typeof(int);
            object @default = 0;

            // Act
            DataTypeConfiguration dataTypeConfiguration = DataTypeConfiguration.Create(dataType, type, @default);

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
            DataTypes dataType = DataTypes.Integer;
            Type type = null;
            object @default = 0;

            // Act
            ArgumentNullException argumentNullException = Assert.Throws<ArgumentNullException>(() => DataTypeConfiguration.Create(dataType, type, @default));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(type));
        }
    }
}
