namespace Rules.Framework.Builder
{
    internal sealed class ConditionNodeBuilder<TConditionType> : IConditionNodeBuilder<TConditionType>
    {
        //[Obsolete("This way of defining conditions has been deprecated. Please use Value(), Or() or And() methods.")]
        public IComposedConditionNodeBuilder<TConditionType> AsComposed()
            => new ComposedConditionNodeBuilder<TConditionType>(this);

        //[Obsolete("This way of defining conditions has been deprecated. Please use Value(), Or() or And() methods.")]
        public IValueConditionNodeBuilder<TConditionType> AsValued(TConditionType conditionType)
            => new ValueConditionNodeBuilder<TConditionType>(conditionType);
    }
}