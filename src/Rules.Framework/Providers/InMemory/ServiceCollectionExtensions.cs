namespace Microsoft.Extensions.DependencyInjection
{
    using Rules.Framework.Providers.InMemory;

    /// <summary>
    /// Service collection extensions from in-memory provider.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the in memory rules data source with specified service lifetime.
        /// </summary>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="serviceLifetime">The service lifetime.</param>
        /// <returns></returns>
        public static IServiceCollection AddInMemoryRulesDataSource(this IServiceCollection serviceDescriptors, ServiceLifetime serviceLifetime)
        {
            ServiceDescriptor item = ServiceDescriptor.Describe(
                typeof(IInMemoryRulesStorage),
                (sp) => new InMemoryRulesStorage(),
                serviceLifetime);
            serviceDescriptors.Add(item);

            return serviceDescriptors;
        }
    }
}