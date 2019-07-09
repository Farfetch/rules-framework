namespace Rules.Framework.IntegrationTests.DataSource
{
    using System.Collections.Generic;

    internal class ConditionNodeDataModel
    {
        public IEnumerable<ConditionNodeDataModel> ChildConditionNodes { get; set; }

        public short ConditionTypeCode { get; set; }

        public short DataTypeCode { get; set; }

        public short LogicalOperatorCode { get; set; }

        public string Operand { get; set; }

        public short OperatorCode { get; set; }
    }
}