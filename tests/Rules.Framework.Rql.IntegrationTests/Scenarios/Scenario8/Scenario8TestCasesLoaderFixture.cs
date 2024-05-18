namespace Rules.Framework.Rql.IntegrationTests.Scenarios.Scenario8
{
    using System.Collections.Generic;
    using System.Reflection;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public static class Scenario8TestCasesLoaderFixture
    {
        private const string testCasesFile = "Rules.Framework.Rql.IntegrationTests.Scenarios.Scenario8.TestCases.yaml";

        static Scenario8TestCasesLoaderFixture()
        {
            var scenarioTestCases = LoadScenarioTestCases();
            MatchAllTestCases = scenarioTestCases.MatchAllTestCases.Select(tc => new object[] { tc.Rql!, tc.ExpectsRules, tc.RuleNames! }).ToList();
            MatchOneTestCases = scenarioTestCases.MatchOneTestCases.Select(tc => new object[] { tc.Rql!, tc.ExpectsRule, tc.RuleName! }).ToList();
            SearchTestCases = scenarioTestCases.SearchTestCases.Select(tc => new object[] { tc.Rql!, tc.ExpectsRules, tc.RuleNames! }).ToList();
        }

        public static IEnumerable<object[]>? MatchAllTestCases { get; private set; }

        public static IEnumerable<object[]>? MatchOneTestCases { get; private set; }

        public static IEnumerable<object[]>? SearchTestCases { get; private set; }

        private static RqlScenarioTestCases LoadScenarioTestCases()
        {
            var testCasesStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(testCasesFile);
            using (var testCasesStreamReader = new StreamReader(testCasesStream!))
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                return deserializer.Deserialize<RqlScenarioTestCases>(testCasesStreamReader);
            }
        }
    }
}