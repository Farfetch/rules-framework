namespace Rules.Framework.WebUI.ViewModels
{
    public sealed class ValueConditionNodeViewModel : ConditionNodeViewModel
    {
        internal ValueConditionNodeViewModel()
        {
        }

        public string Condition { get; internal set; }

        public string DataType { get; internal set; }

        public dynamic Operand { get; internal set; }

        public string Operator { get; internal set; }
    }
}