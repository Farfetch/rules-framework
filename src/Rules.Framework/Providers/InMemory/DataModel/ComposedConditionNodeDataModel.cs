namespace Rules.Framework.Providers.InMemory.DataModel
{
    internal sealed class ComposedConditionNodeDataModel<TConditionType> : ConditionNodeDataModel<TConditionType>
    {
        public ConditionNodeDataModel<TConditionType>[] ChildConditionNodes { get; set; }
    }
}