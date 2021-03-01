namespace Rules.Framework.Providers.InMemory.DataModel
{
    using Rules.Framework.Core;

    internal class ConditionNodeDataModel<TConditionType>
    {
        public LogicalOperators LogicalOperator { get; set; }
    }
}