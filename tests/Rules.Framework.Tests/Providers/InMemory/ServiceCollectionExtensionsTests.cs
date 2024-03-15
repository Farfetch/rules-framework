namespace Rules.Framework.Tests.Providers.InMemory
{
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Rules.Framework.Tests.Providers.InMemory.TestStubs;
    using Xunit;

    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddInMemoryRulesDataSource_GivenSingletonLifetimeOption_AddsServiceDescriptorAsSingleton()
        {
            // Arrange
            var serviceLifetime = ServiceLifetime.Singleton;
            var services = Mock.Of<IServiceCollection>();

            ServiceDescriptor serviceDescriptor = null;
            Mock.Get(services)
                .Setup(x => x.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(sd =>
                {
                    serviceDescriptor = sd;
                });

            // Act
            var actual = services.AddInMemoryRulesDataSource<ContentType, ConditionType>(serviceLifetime);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().BeSameAs(services);
            serviceDescriptor.Should().NotBeNull();
            serviceDescriptor.Lifetime.Should().Be(serviceLifetime);
        }
    }
}