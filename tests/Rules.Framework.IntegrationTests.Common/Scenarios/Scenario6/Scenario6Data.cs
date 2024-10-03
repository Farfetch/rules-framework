namespace Rules.Framework.BenchmarkTests.Tests.Benchmark1
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.BenchmarkTests.Tests;
    using Rules.Framework.Generic;

    public class Scenario6Data : IScenarioData<Rulesets, ConditionNames>
    {
        public IEnumerable<Condition<ConditionNames>> Conditions => new[]
        {
            new Condition<ConditionNames>(ConditionNames.StringCondition, "Let's benchmark this!")
        };

        public DateTime MatchDate => DateTime.Parse("2022-10-01");

        public IEnumerable<Rule<Rulesets, ConditionNames>> Rules => this.GetRules();

        private IEnumerable<Rule<Rulesets, ConditionNames>> GetRules()
        {
            var ruleResult = Rule.Create<Rulesets, ConditionNames>("Benchmark 1 - Test rule")
                .InRuleset(Rulesets.Sample1)
                .SetContent("Dummy Content")
                .Since(DateTime.Parse("2000-01-01"))
                .ApplyWhen(ConditionNames.StringCondition, Operators.Equal, "Let's benchmark this!")
                .Build();

            return new[] { ruleResult.Rule };
        }
    }
}