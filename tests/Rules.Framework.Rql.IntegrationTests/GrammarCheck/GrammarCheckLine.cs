namespace Rules.Framework.Rql.IntegrationTests.GrammarCheck
{
    internal class GrammarCheckLine
    {
        public string[] ExpectedMessages { get; init; }
        public bool ExpectsSuccess { get; init; }
        public string Rql { get; init; }
        public string[] Tags { get; init; }
    }
}