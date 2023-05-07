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
                .WithCondition(x =>
                {
                    return x.AsComposed()
                        .WithLogicalOperator(LogicalOperators.And)
                        .AddCondition(c =>
                            c.AsValued(ConditionTypes.Artist)
                                .OfDataType<string>()
                                .WithComparisonOperator(Operators.Equal)
                                .SetOperand("Queen")
                                .Build())
                        .AddCondition(c =>
                            c.AsValued(ConditionTypes.Lyrics)
                                .OfDataType<string>()
                                .WithComparisonOperator(Operators.Contains)
                                .SetOperand("real life")
                                .Build())
                        .AddCondition(c =>
                            c.AsValued(ConditionTypes.ReleaseYear)
                                .OfDataType<int>()
                                .WithComparisonOperator(Operators.GreaterThanOrEqual)
                                .SetOperand(1973)
                                .Build())
                        .AddCondition(c =>
                            c.AsValued(ConditionTypes.ReleaseYear)
                                .OfDataType<int>()
                                .WithComparisonOperator(Operators.GreaterThanOrEqual)
                                .SetOperand(1977)
                                .Build())
                        .Build();
                })
                .Build();

            var rule2Result = RuleBuilder.NewRule<ContentTypes, ConditionTypes>()
                .WithName("Benchmark 2 - Stairway to Heaven")
                .WithDateBegin(DateTime.Parse("2000-01-01"))
                .WithContent(ContentTypes.Songs, "Stairway to Heaven")
                .WithCondition(x =>
                {
                    return x.AsComposed()
                        .WithLogicalOperator(LogicalOperators.And)
                        .AddCondition(c =>
                            c.AsValued(ConditionTypes.Artist)
                                .OfDataType<string>()
                                .WithComparisonOperator(Operators.Equal)
                                .SetOperand("Led Zeppelin")
                                .Build())
                        .AddCondition(c =>
                            c.AsComposed()
                                .WithLogicalOperator(LogicalOperators.Or)
                                .AddCondition(y =>
                                    y.AsValued(ConditionTypes.Lyrics)
                                        .OfDataType<string>()
                                        .WithComparisonOperator(Operators.Contains)
                                        .SetOperand("all that glitters is gold")
                                        .Build())
                                .AddCondition(y =>
                                    y.AsValued(ConditionTypes.Lyrics)
                                        .OfDataType<string>()
                                        .WithComparisonOperator(Operators.Contains)
                                        .SetOperand("it makes me wonder")
                                        .Build())
                                .Build())
                        .AddCondition(c =>
                            c.AsValued(ConditionTypes.ReleaseYear)
                                .OfDataType<int>()
                                .WithComparisonOperator(Operators.GreaterThanOrEqual)
                                .SetOperand(1973)
                                .Build())
                        .AddCondition(c =>
                            c.AsValued(ConditionTypes.ReleaseYear)
                                .OfDataType<int>()
                                .WithComparisonOperator(Operators.GreaterThanOrEqual)
                                .SetOperand(1977)
                                .Build())
                        .Build();
                })
                .Build();

            return new[] { rule1Result.Rule, rule2Result.Rule };
        }
    }
}