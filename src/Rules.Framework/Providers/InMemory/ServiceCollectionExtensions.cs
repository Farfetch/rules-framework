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
        /// <typeparam name="TContentType">The type of the content type.</typeparam>
        /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
        /// <param name="serviceDescriptors">The service descriptors.</param>
        /// <param name="serviceLifetime">The service lifetime.</param>
        /// <returns></returns>
        public static IServiceCollection AddInMemoryRulesDataSource<TContentType, TConditionType>(this IServiceCollection serviceDescriptors, ServiceLifetime serviceLifetime)
        {
            ServiceDescriptor item = ServiceDescriptor.Describe(
                typeof(IInMemoryRulesStorage<TContentType, TConditionType>),
                (sp) => new InMemoryRulesStorage<TContentType, TConditionType>(),
                serviceLifetime);
            serviceDescriptors.Add(item);

            return serviceDescriptors;
        }
    }
}