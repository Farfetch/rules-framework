namespace Rules.Framework.IntegrationTests.DataSource
{
    using System.Collections.Generic;

    internal class ConditionNodeDataModel
    {
        public IEnumerable<ConditionNodeDataModel> ChildConditionNodes { get; set; }

        public string Condition { get; set; }

        public string DataType { get; set; }

        public string LogicalOperator { get; set; }

        public string Operand { get; set; }

        public string Operator { get; set; }
    }
}