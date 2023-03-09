namespace Rules.Framework.Providers.InMemory.DataModel
{
    using System.Collections.Generic;

    internal sealed class ComposedConditionNodeDataModel<TConditionType> : ConditionNodeDataModel<TConditionType>
    {
        public IEnumerable<ConditionNodeDataModel<TConditionType>> ChildConditionNodes { get; set; }
    }
}