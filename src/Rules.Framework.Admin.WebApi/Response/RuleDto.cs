namespace Rules.Framework.Admin.WebApi.Response
{
    public class RuleDto
    {
        public bool Active { get; set; }
        public string Conditions { get; set; }
        public string DateEnd { get; set; }
        public string DateStart { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}