namespace Rules.Framework.Builder
{
    using System;

    /// <summary>
    /// Builder to specify a new condition node. Allows for choosing between composed and valued
    /// condition nodes.
    /// </summary>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    [Obsolete("This way of building conditions has been deprecated. Please use the IRootConditionNodeBuilder and IFluentConditionNodeBuilder interfaces.")]
    public interface IConditionNodeBuilder<TConditionType>
    {
        /// <summary>
        /// Begins building a condition node as composed.
        /// </summary>
        /// <returns></returns>
        //[Obsolete("This way of building conditions has been deprecated. Please use the other options available.")]
        IComposedConditionNodeBuilder<TConditionType> AsComposed();

        /// <summary>
        /// Begins building a condition node as valued.
        /// </summary>
        /// <param name="conditionType">Type of the condition.</param>
        /// <returns></returns>
        //[Obsolete("This way of building conditions has been deprecated. Please use the other options available.")]
        IValueConditionNodeBuilder<TConditionType> AsValued(TConditionType conditionType);
    }
}