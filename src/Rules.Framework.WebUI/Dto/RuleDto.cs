namespace Rules.Framework.WebUI.Dto
{
    internal sealed class RuleDto
    {
        public ConditionNodeDto Conditions { get; internal set; }
        public string DateBegin { get; internal set; }
        public string DateEnd { get; internal set; }
        public string Name { get; internal set; }
        public int? Priority { get; internal set; }
        public string Ruleset { get; internal set; }
        public string Status { get; internal set; }
        public object Value { get; internal set; }
    }
}