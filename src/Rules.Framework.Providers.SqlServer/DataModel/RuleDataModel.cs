namespace Rules.Framework.Providers.SqlServer.DataModel
{
    using System;

    public class RuleDataModel
    {
        public long? ConditionNodeId { get; set; }
        public dynamic Content { get; set; }
        public string ContentType { get; set; }
        public int ContentTypeCode { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public ConditionNodeDataModel RootCondition { get; set; }
    }
}