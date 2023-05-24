namespace Rules.Framework.BenchmarkTests.Tests.Benchmark2
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;

    public class Scenario7Data : IScenarioData<ContentTypes, ConditionTypes>
    {
        public IEnumerable<Condition<ConditionTypes>> Conditions => new[]
        {
            new Condition<ConditionTypes> { Type = ConditionTypes.Artist, Value = "Queen" },
            new Condition<ConditionTypes> { Type = ConditionTypes.Lyrics, Value = "Is this the real life?\nIs this just fantasy?\nCaught in a landside,\nNo escape from reality" },
            new Condition<ConditionTypes> { Type = ConditionTypes.ReleaseYear, Value = 1975 }
        };

        public DateTime MatchDate => DateTime.Parse("2022-11-01");

        public IEnumerable<Rule<ContentTypes, ConditionTypes>> Rules => this.GetRules();

        private IEnumerable<Rule<ContentTypes, ConditionTypes>> GetRules()
        {
            var rule1Result = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Benchmark 2 - Bohemian Rapsody")
                .WithDateBegin(DateTime.Parse("2000-01-01"))
                .WithContent(ContentTypes.Songs, "Bohemian Rapsody")
                .WithCondition(c => c
                    .And(x => x
                        .Value(ConditionTypes.Artist, Operators.Equal, "Queen")
                        .Value(ConditionTypes.Lyrics, Operators.Contains, "real life")
                        .Value(ConditionTypes.ReleaseYear, Operators.GreaterThanOrEqual, 1973)
                        .Value(ConditionTypes.ReleaseYear, Operators.GreaterThanOrEqual, 1977)
                    )
                )
                .Build();

            var rule2Result = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Benchmark 2 - Stairway to Heaven")
                .WithDateBegin(DateTime.Parse("2000-01-01"))
                .WithContent(ContentTypes.Songs, "Stairway to Heaven")
                .WithCondition(c => c
                    .And(x => x
                        .Value(ConditionTypes.Artist, Operators.Equal, "Led Zeppelin")
                        .Or(sub => sub
                            .Value(ConditionTypes.Lyrics, Operators.Contains, "all that glitters is gold")
                            .Value(ConditionTypes.Lyrics, Operators.Contains, "it makes me wonder")
                        )
                        .Value(ConditionTypes.ReleaseYear, Operators.GreaterThanOrEqual, 1973)
                        .Value(ConditionTypes.ReleaseYear, Operators.GreaterThanOrEqual, 1977)
                    )
                )
                .Build();

            return new[] { rule1Result.Rule, rule2Result.Rule };
        }
    }
}