namespace Rules.Framework.WebUI.ViewModels
{
    internal sealed class ValueConditionNodeViewModel : ConditionNodeViewModel
    {
        public string Condition { get; internal set; }

        public string DataType { get; internal set; }

        public dynamic Operand { get; internal set; }

        public string Operator { get; internal set; }
    }
}