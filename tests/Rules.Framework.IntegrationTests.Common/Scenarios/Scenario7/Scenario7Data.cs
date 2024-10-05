namespace Rules.Framework.BenchmarkTests.Tests.Benchmark2
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.Generic;

    public class Scenario7Data : IScenarioData<Rulesets, ConditionNames>
    {
        public IEnumerable<Condition<ConditionNames>> Conditions => new[]
        {
            new Condition<ConditionNames>(ConditionNames.Artist, "Queen"),
            new Condition<ConditionNames>(ConditionNames.Lyrics, "Is this the real life?\nIs this just fantasy?\nCaught in a landside,\nNo escape from reality" ),
            new Condition<ConditionNames>(ConditionNames.ReleaseYear, 1975 )
        };

        public DateTime MatchDate => DateTime.Parse("2022-11-01");

        public IEnumerable<Rule<Rulesets, ConditionNames>> Rules => this.GetRules();

        private IEnumerable<Rule<Rulesets, ConditionNames>> GetRules()
        {
            var rule1Result = Rule.Create<Rulesets, ConditionNames>("Benchmark 2 - Bohemian Rapsody")
                .InRuleset(Rulesets.Songs)
                .SetContent("Bohemian Rapsody")
                .SinceUtc(2000, 1, 1)
                .ApplyWhen(c => c
                    .And(x => x
                        .Value(ConditionNames.Artist, Operators.Equal, "Queen")
                        .Value(ConditionNames.Lyrics, Operators.Contains, "real life")
                        .Value(ConditionNames.ReleaseYear, Operators.GreaterThanOrEqual, 1973)
                        .Value(ConditionNames.ReleaseYear, Operators.GreaterThanOrEqual, 1977)
                    )
                )
                .Build();

            var rule2Result = Rule.Create<Rulesets, ConditionNames>("Benchmark 2 - Stairway to Heaven")
                .InRuleset(Rulesets.Songs)
                .SetContent("Stairway to Heaven")
                .SinceUtc(2000, 1, 1)
                .ApplyWhen(c => c
                    .And(x => x
                        .Value(ConditionNames.Artist, Operators.Equal, "Led Zeppelin")
                        .Or(sub => sub
                            .Value(ConditionNames.Lyrics, Operators.Contains, "all that glitters is gold")
                            .Value(ConditionNames.Lyrics, Operators.Contains, "it makes me wonder")
                        )
                        .Value(ConditionNames.ReleaseYear, Operators.GreaterThanOrEqual, 1973)
                        .Value(ConditionNames.ReleaseYear, Operators.GreaterThanOrEqual, 1977)
                    )
                )
                .Build();

            return new[] { rule1Result.Rule, rule2Result.Rule };
        }
    }
}