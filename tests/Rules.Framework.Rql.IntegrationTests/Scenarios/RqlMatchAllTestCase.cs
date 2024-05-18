namespace Rules.Framework.Rql.IntegrationTests.Scenarios
{
    internal class RqlMatchAllTestCase
    {
        public bool ExpectsRules { get; set; }
        public string? Rql { get; set; }
        public string[]? RuleNames { get; set; }
    }
}