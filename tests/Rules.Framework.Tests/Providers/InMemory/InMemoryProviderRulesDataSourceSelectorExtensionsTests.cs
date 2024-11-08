namespace Rules.Framework.Tests.Providers.InMemory
{
    using System;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Rules.Framework.Builder;
    using Rules.Framework.Providers.InMemory;
    using Xunit;

    public class InMemoryProviderRulesDataSourceSelectorExtensionsTests
    {
        [Fact]
        public void SetInMemoryDataSource_GivenNullRulesDataSourceSelector_ThrowsArgumentNullException()
        {
            // Arrange
            IRulesDataSourceSelector rulesDataSourceSelector = null;

            // Act
            var actual = Assert.Throws<ArgumentNullException>(() => rulesDataSourceSelector.SetInMemoryDataSource());

            // Assert
            actual.Should().NotBeNull();
            actual.ParamName.Should().Be(nameof(rulesDataSourceSelector));
        }

        [Fact]
        public void SetInMemoryDataSource_GivenNullServiceProvider_ThrowsArgumentNullException()
        {
            // Arrange
            IServiceProvider serviceProvider = null;

            var rulesDataSourceSelector = Mock.Of<IRulesDataSourceSelector>();

            // Act
            var actual = Assert.Throws<ArgumentNullException>(() => rulesDataSourceSelector.SetInMemoryDataSource(serviceProvider));

            // Assert
            actual.Should().NotBeNull();
            actual.ParamName.Should().Be(nameof(serviceProvider));
        }

        [Fact]
        public void SetInMemoryDataSource_GivenServiceProvider_RequestsInMemoryRulesStorageAndSetsOnSelector()
        {
            // Arrange
            var inMemoryRulesStorage = new InMemoryRulesStorage();

            var serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddSingleton<IInMemoryRulesStorage>(inMemoryRulesStorage);
            var serviceProvider = serviceDescriptors.BuildServiceProvider();

            var rulesDataSourceSelector = Mock.Of<IRulesDataSourceSelector>();

            IRulesDataSource actualRulesDataSource = null;
            Mock.Get(rulesDataSourceSelector)
                .Setup(x => x.SetDataSource(It.IsAny<IRulesDataSource>()))
                .Callback<IRulesDataSource>((rds) =>
                {
                    actualRulesDataSource = rds;
                });

            // Act
            rulesDataSourceSelector.SetInMemoryDataSource(serviceProvider);

            // Assert
            actualRulesDataSource.Should().NotBeNull();
            actualRulesDataSource.Should().BeOfType<InMemoryProviderRulesDataSource>();
            Mock.Get(rulesDataSourceSelector)
                .Verify();
        }

        [Fact]
        public void SetInMemoryDataSource_NoParametersGiven_CreatesTransientInMemoryRulesStorageAndSetsOnSelector()
        {
            // Arrange
            var rulesDataSourceSelector = Mock.Of<IRulesDataSourceSelector>();

            IRulesDataSource actualRulesDataSource = null;
            Mock.Get(rulesDataSourceSelector)
                .Setup(x => x.SetDataSource(It.IsAny<IRulesDataSource>()))
                .Callback<IRulesDataSource>((rds) =>
                {
                    actualRulesDataSource = rds;
                });

            // Act
            rulesDataSourceSelector.SetInMemoryDataSource();

            // Assert
            actualRulesDataSource.Should().NotBeNull();
            actualRulesDataSource.Should().BeOfType<InMemoryProviderRulesDataSource>();
            Mock.Get(rulesDataSourceSelector)
                .Verify();
        }
    }
}