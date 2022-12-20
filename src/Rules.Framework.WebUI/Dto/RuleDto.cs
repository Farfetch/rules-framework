namespace Rules.Framework.WebUI.Dto
{
    internal sealed class RuleDto
    {
        public object Conditions { get; set; }
        public string DateBegin { get; set; }
        public string DateEnd { get; set; }
        public string Name { get; set; }
        public int? Priority { get; set; }
        public string Status { get; set; }
        public object Value { get; set; }
    }
}