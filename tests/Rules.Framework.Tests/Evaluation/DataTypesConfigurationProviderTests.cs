namespace Rules.Framework.Tests.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Rules.Framework.Core;
    using Rules.Framework.Evaluation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class DataTypesConfigurationProviderTests
    {
        public static IEnumerable<object[]> DataTypes => Enum.GetValues<DataTypes>()
            .Select(dt => new object[] { dt })
            .ToArray();

        [Theory]
        [MemberData(nameof(DataTypes))]
        public void GetDataTypeConfiguration_GivenMappedDataType_ReturnsDataTypeConfiguration(DataTypes dataType)
        {
            // Arrange
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var dataTypesConfigurationProvider = new DataTypesConfigurationProvider(rulesEngineOptions);

            // Act
            var dataTypeConfiguration = dataTypesConfigurationProvider.GetDataTypeConfiguration(dataType);

            // Assert
            dataTypeConfiguration.Should().NotBeNull();
            dataTypeConfiguration.DataType.Should().Be(dataType);
        }

        [Fact]
        public void GetDataTypeConfiguration_GivenUnkownDataType_ThrowsNotSupportedException()
        {
            // Arrange
            var dataType = (DataTypes)0;
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var dataTypesConfigurationProvider = new DataTypesConfigurationProvider(rulesEngineOptions);

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => dataTypesConfigurationProvider.GetDataTypeConfiguration(dataType));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain($"{dataType}");
        }
    }
}