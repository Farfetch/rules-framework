namespace Rules.Framework.Builder
{
    internal sealed class ConditionNodeBuilder<TConditionType> : IConditionNodeBuilder<TConditionType>
    {
        public IComposedConditionNodeBuilder<TConditionType> AsComposed()
            => new ComposedConditionNodeBuilder<TConditionType>(this);

        public IValueConditionNodeBuilder<TConditionType> AsValued(TConditionType conditionType)
            => new ValueConditionNodeBuilder<TConditionType>(conditionType);
    }
}