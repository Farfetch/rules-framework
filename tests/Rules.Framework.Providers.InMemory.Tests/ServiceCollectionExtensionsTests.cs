namespace Rules.Framework.Providers.InMemory.Tests
{
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Rules.Framework.Providers.InMemory.Tests.TestStubs;
    using Xunit;

    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddInMemoryRulesDataSource_GivenSingletonLifetimeOption_AddsServiceDescriptorAsSingleton()
        {
            // Arrange
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton;
            IServiceCollection services = Mock.Of<IServiceCollection>();

            ServiceDescriptor serviceDescriptor = null;
            Mock.Get(services)
                .Setup(x => x.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(sd =>
                {
                    serviceDescriptor = sd;
                });

            // Act
            IServiceCollection actual = services.AddInMemoryRulesDataSource<ContentType, ConditionType>(serviceLifetime);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().BeSameAs(services);
            serviceDescriptor.Should().NotBeNull();
            serviceDescriptor.Lifetime.Should().Be(serviceLifetime);
        }
    }
}