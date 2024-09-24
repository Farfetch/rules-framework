namespace Rules.Framework.BenchmarkTests.Tests.Benchmark1
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.BenchmarkTests.Tests;
    using Rules.Framework.Generic;

    public class Scenario6Data : IScenarioData<ContentTypes, ConditionTypes>
    {
        public IEnumerable<Condition<ConditionTypes>> Conditions => new[]
        {
            new Condition<ConditionTypes>(ConditionTypes.StringCondition, "Let's benchmark this!")
        };

        public DateTime MatchDate => DateTime.Parse("2022-10-01");

        public IEnumerable<Rule<ContentTypes, ConditionTypes>> Rules => this.GetRules();

        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetRules()
        {
            var ruleResult = Rule.New<ContentTypes, ConditionTypes>()
                .WithName("Benchmark 1 - Test rule")
                .WithDateBegin(DateTime.Parse("2000-01-01"))
                .WithContent(ContentTypes.ContentType1, "Dummy Content")
                .WithCondition(ConditionTypes.StringCondition, Operators.Equal, "Let's benchmark this!")
                .Build();

            return new[] { ruleResult.Rule };
        }
    }
}