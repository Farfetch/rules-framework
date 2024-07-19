namespace Rules.Framework.Providers.InMemory.DataModel
{
    internal sealed class ComposedConditionNodeDataModel : ConditionNodeDataModel
    {
        public ConditionNodeDataModel[] ChildConditionNodes { get; set; }
    }
}