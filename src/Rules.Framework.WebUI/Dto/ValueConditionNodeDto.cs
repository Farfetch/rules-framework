namespace Rules.Framework.WebUI.Dto
{
    internal sealed class ValueConditionNodeDto : ConditionNodeDto
    {
        public string Condition { get; internal set; }

        public string DataType { get; internal set; }

        public dynamic Operand { get; internal set; }

        public string Operator { get; internal set; }
    }
}