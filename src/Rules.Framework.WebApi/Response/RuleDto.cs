namespace Rules.Framework.WebApi.Response
{
    public class RuleDto
    {
        public string? Conditions { get; set; }
        public string? DateBegin { get; set; }
        public string? DateEnd { get; set; }
        public string? Name { get; set; }
        public int? Priority { get; set; }
        public string? Status { get; set; }
        public string? Value { get; set; }
    }
}