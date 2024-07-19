namespace Rules.Framework.WebUI.Dto
{
    using System;

    internal sealed class RulesFilterDto
    {
        public string Content { get; set; }
        public DateTime? DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Name { get; set; }
        public string Ruleset { get; set; }
        public RuleStatusDto? Status { get; set; }
    }
}