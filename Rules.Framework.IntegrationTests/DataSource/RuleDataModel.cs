using System;

namespace Rules.Framework.IntegrationTests.DataSource
{
    internal class RuleDataModel
    {
        public string Content { get; set; }

        public short ContentTypeCode { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public string Name { get; set; }

        public int Priority { get; set; }

        public ConditionNodeDataModel RootCondition { get; set; }
    }
}