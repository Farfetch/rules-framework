namespace Rules.Framework.Providers.InMemory.DataModel
{
    using System.Collections.Generic;
    using Rules.Framework.Core;

    internal class ConditionNodeDataModel<TConditionType>
    {
        public LogicalOperators LogicalOperator { get; set; }

        public IDictionary<string, object> Properties { get; set; }
    }
}