namespace Rules.Framework.Tests.Evaluation
{
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
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            DataTypesConfigurationProvider dataTypesConfigurationProvider = new DataTypesConfigurationProvider(rulesEngineOptions);

            // Act
            DataTypeConfiguration dataTypeConfiguration = dataTypesConfigurationProvider.GetDataTypeConfiguration(dataType);

            // Assert
            dataTypeConfiguration.Should().NotBeNull();
            dataTypeConfiguration.DataType.Should().Be(dataType);
        }

        [Fact]
        public void GetDataTypeConfiguration_GivenUnkownDataType_ThrowsNotSupportedException()
        {
            // Arrange
            DataTypes dataType = (DataTypes)0;
            RulesEngineOptions rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            DataTypesConfigurationProvider dataTypesConfigurationProvider = new DataTypesConfigurationProvider(rulesEngineOptions);

            // Act
            NotSupportedException notSupportedException = Assert.Throws<NotSupportedException>(() => dataTypesConfigurationProvider.GetDataTypeConfiguration(dataType));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain($"{dataType}");
        }
    }
}
