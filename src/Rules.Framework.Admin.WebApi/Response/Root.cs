namespace Rules.Framework.Admin.WebApi.Response
{
    public class ChildConditionNode
    {
        public List<ChildConditionNode> ChildConditionNodes { get; set; }
        public int ConditionType { get; set; }
        public int DataType { get; set; }
        public int LogicalOperator { get; set; }
        public string Operand { get; set; }
        public int Operator { get; set; }
    }

    public class ContentContainer
    {
        public int ContentType { get; set; }
    }

    public class Root
    {
        public ContentContainer ContentContainer { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public RootCondition RootCondition { get; set; }
    }

    public class RootCondition
    {
        public List<ChildConditionNode> ChildConditionNodes { get; set; }
        public int LogicalOperator { get; set; }
    }
}