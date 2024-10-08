namespace Rules.Framework.Providers.InMemory.DataModel
{
    using System.Collections.Generic;
    using Rules.Framework;

    internal class ConditionNodeDataModel
    {
        public LogicalOperators LogicalOperator { get; set; }

        public IDictionary<string, object> Properties { get; set; }
    }
}