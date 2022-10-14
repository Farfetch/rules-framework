namespace Rules.Framework.Providers.SqlServer.DataModel
{
    using System.Collections.Generic;

    internal class ComposedConditionNodeDataModel : ConditionNodeDataModel
    {
        public IEnumerable<ConditionNodeDataModel> ChildConditionNodes { get; set; }
    }
}