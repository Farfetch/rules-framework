namespace Rules.Framework.Admin.WebApi.Response
{
    public class RuleDto
    {
        public bool Active { get; set; }
        public IEnumerable<ConditionDto> Conditions { get; set; }
        public DateTime? DateEnd { get; set; }
        public DateTime DateStart { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}