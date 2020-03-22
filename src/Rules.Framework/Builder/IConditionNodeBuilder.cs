namespace Rules.Framework.Builder
{
    /// <summary>
    /// Builder to specify a new condition node. Allows for choosing between composed and valued condition nodes.
    /// </summary>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    public interface IConditionNodeBuilder<TConditionType>
    {
        /// <summary>
        /// Begins building a condition node as composed.
        /// </summary>
        /// <returns></returns>
        IComposedConditionNodeBuilder<TConditionType> AsComposed();

        /// <summary>
        /// Begins building a condition node as valued.
        /// </summary>
        /// <param name="conditionType">Type of the condition.</param>
        /// <returns></returns>
        IValueConditionNodeBuilder<TConditionType> AsValued(TConditionType conditionType);
    }
}