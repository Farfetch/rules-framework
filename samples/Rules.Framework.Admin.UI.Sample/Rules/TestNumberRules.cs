namespace Rules.Framework.Admin.UI.Sample.Rules
{
    using System;
    using System.Collections.Generic;
    using global::Rules.Framework.Admin.UI.Sample.Engine;
    using global::Rules.Framework.Admin.UI.Sample.Enums;
    using global::Rules.Framework.Builder;
    using global::Rules.Framework.Core;

    internal class TestNumberRules : IContentTypes
    {
        public readonly List<RuleSpecification> rulesSpecifications;

        public TestNumberRules()
        {
            this.rulesSpecifications = new List<RuleSpecification>();
        }

        public IEnumerable<RuleSpecification> GetRulesSpecifications()
        {
            Add(CreateRuleForCoolNumbers(), RuleAddPriorityOption.ByPriorityNumber(3));
            Add(CreateRuleForSosoNumbers(), RuleAddPriorityOption.ByPriorityNumber(2));
            Add(CreateDefaultRule(), RuleAddPriorityOption.ByPriorityNumber(1));

            return this.rulesSpecifications;
        }

        private void Add(
            RuleBuilderResult<ContentTypes, ConditionTypes> rule,
            RuleAddPriorityOption ruleAddPriorityOption) => rulesSpecifications.Add(
                new RuleSpecification
                {
                    RuleBuilderResult = rule,
                    RuleAddPriorityOption = ruleAddPriorityOption,
                });

        private RuleBuilderResult<ContentTypes, ConditionTypes> CreateDefaultRule() =>
                                     RuleBuilder
                             .NewRule<ContentTypes, ConditionTypes>()
                             .WithName("Default rule for test number")
                             .WithContent(ContentTypes.TestNumber, ":| default nothing special about this number")
                             .WithDatesInterval(new DateTime(2019, 01, 01), new DateTime(2022, 10, 01))
                             .Build();

        private RuleBuilderResult<ContentTypes, ConditionTypes> CreateRuleForCoolNumbers() =>
                                     RuleBuilder
                             .NewRule<ContentTypes, ConditionTypes>()
                             .WithName("Rule for cool numbers")
                             .WithContent(ContentTypes.TestNumber, ":D this number is so COOL!")
                             .WithDateBegin(new DateTime(2019, 01, 01))
                             .WithCondition(cnb => cnb.AsComposed()
                                    .WithLogicalOperator(LogicalOperators.Or)
                                        .AddCondition(condition => condition
                                            .AsValued(ConditionTypes.RoyalNumber).OfDataType<int>()
                                            .WithComparisonOperator(Operators.Equal)
                                            .SetOperand(7)
                                            .Build())
                                        .AddCondition(condition => condition.AsComposed()
                                            .WithLogicalOperator(LogicalOperators.And)
                                            .AddCondition(sub => sub
                                                .AsValued(ConditionTypes.IsPrimeNumber).OfDataType<bool>()
                                                .WithComparisonOperator(Operators.Equal)
                                                .SetOperand(true)
                                                .Build())
                                            .AddCondition(sub => sub
                                                .AsValued(ConditionTypes.SumAll).OfDataType<string>()
                                                .WithComparisonOperator(Operators.StartsWith)
                                                .SetOperand("5")
                                                .Build())
                                            .Build())
                                        .Build())
                              .Build();

        private RuleBuilderResult<ContentTypes, ConditionTypes> CreateRuleForSosoNumbers() =>
                                     RuleBuilder
                             .NewRule<ContentTypes, ConditionTypes>()
                             .WithName("Rule for so so numbers")
                             .WithContent(ContentTypes.TestNumber, ":) this number is so so")
                             .WithDatesInterval(new DateTime(2023, 01, 01), new DateTime(2023, 10, 01))
                             .WithCondition(cnb => cnb.AsComposed()
                                    .WithLogicalOperator(LogicalOperators.Or)
                                        .AddCondition(condition => condition
                                            .AsValued(ConditionTypes.CanNumberBeDividedBy3).OfDataType<bool>()
                                            .WithComparisonOperator(Operators.Equal)
                                            .SetOperand(true)
                                            .Build())
                                        .AddCondition(condition => condition
                                            .AsValued(ConditionTypes.IsPrimeNumber).OfDataType<bool>()
                                            .WithComparisonOperator(Operators.Equal)
                                            .SetOperand(false)
                                            .Build())
                                        .AddCondition(sub => sub
                                            .AsValued(ConditionTypes.SumAll).OfDataType<string>()
                                            .WithComparisonOperator(Operators.EndsWith)
                                            .SetOperand("9")
                                            .Build())
                                        .Build())
                              .Build();
    }
}