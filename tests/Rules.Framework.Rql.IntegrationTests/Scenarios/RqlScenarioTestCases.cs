namespace Rules.Framework.Rql.IntegrationTests.Scenarios
{
    using System.Collections.Generic;

    internal class RqlScenarioTestCases
    {
        public IEnumerable<RqlMatchAllTestCase> MatchAllTestCases { get; set; }

        public IEnumerable<RqlMatchOneTestCase> MatchOneTestCases { get; set; }

        public IEnumerable<RqlSearchTestCase> SearchTestCases { get; set; }
    }
}