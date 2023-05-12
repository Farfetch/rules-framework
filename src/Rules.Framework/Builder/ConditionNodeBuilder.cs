namespace Rules.Framework.Builder
{
    using System;

    internal sealed class ConditionNodeBuilder<TConditionType> : IConditionNodeBuilder<TConditionType>
    {
        [Obsolete("This way of defining conditions has been deprecated. Please use the other options available.")]
        public IComposedConditionNodeBuilder<TConditionType> AsComposed()
            => new ComposedConditionNodeBuilder<TConditionType>(this);

        [Obsolete("This way of defining conditions has been deprecated. Please use the other options available.")]
        public IValueConditionNodeBuilder<TConditionType> AsValued(TConditionType conditionType)
            => new ValueConditionNodeBuilder<TConditionType>(conditionType);
    }
}