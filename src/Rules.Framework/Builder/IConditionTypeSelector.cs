namespace Rules.Framework.Builder
{
    /// <summary>
    /// Exposes the interface contract for selecting a rules engine condition type.
    /// </summary>
    /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
    public interface IConditionTypeSelector<TContentType>
    {
        /// <summary>
        /// Sets the condition type to use on the set of conditions to supply to rules engine, according to <typeparamref name="TConditionType"/>.
        /// </summary>
        /// <typeparam name="TConditionType">The condition type that allows to filter rules based on a set of conditions.</typeparam>
        /// <returns>a rules data source selector.</returns>
        IRulesDataSourceSelector<TContentType, TConditionType> WithConditionType<TConditionType>();
    }
}