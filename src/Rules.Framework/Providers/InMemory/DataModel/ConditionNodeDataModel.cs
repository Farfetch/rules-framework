namespace Rules.Framework.Providers.InMemory.DataModel
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;

    internal class ConditionNodeDataModel<TConditionType>
    {
        public ConditionNodeDataModel()
        {
            this.Properties = new Dictionary<string, object>(StringComparer.Ordinal);
        }

        public LogicalOperators LogicalOperator { get; set; }

        public IDictionary<string, object> Properties { get; set; }
    }
}