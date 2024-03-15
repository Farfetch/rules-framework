namespace Rules.Framework.Builder
{
    using System;

    [Obsolete("This way of building conditions has been deprecated. Please use the IRootConditionNodeBuilder and IFluentComposedConditionNodeBuilder interfaces.")]
    internal sealed class ConditionNodeBuilder<TConditionType> : IConditionNodeBuilder<TConditionType>
    {
        public IComposedConditionNodeBuilder<TConditionType> AsComposed()
            => new ComposedConditionNodeBuilder<TConditionType>(this);

        public IValueConditionNodeBuilder<TConditionType> AsValued(TConditionType conditionType)
            => new ValueConditionNodeBuilder<TConditionType>(conditionType);
    }
}