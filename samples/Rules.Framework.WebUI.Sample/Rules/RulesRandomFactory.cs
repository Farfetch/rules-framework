namespace Rules.Framework.WebUI.Sample.Rules
{
    using System;
    using System.Collections.Generic;
    using global::Rules.Framework.Builder;
    using global::Rules.Framework.Core;
    using global::Rules.Framework.WebUI.Sample.Engine;
    using global::Rules.Framework.WebUI.Sample.Enums;

    internal class RulesRandomFactory : IContentTypes
    {
        private readonly int finalNumber = 50;
        private readonly int intialNumber = 10;
        private readonly Random random;

        public RulesRandomFactory()
        {
            this.random = new Random();
        }

        public IEnumerable<RuleSpecification> GetRulesSpecifications()
        {
            var currentYear = DateTime.UtcNow.Year;
            var rulesSpecifications = new List<RuleSpecification>();

            foreach (var contentType in Enum.GetValues(typeof(ContentTypes)))
            {
                for (var i = 1; i < random.Next(intialNumber, finalNumber); i++)
                {
                    var dateBegin = CreateRandomDateBegin(currentYear);

                    Add(CreateMultipleRule((ContentTypes)contentType, i, dateBegin, CreateRandomDateEnd(dateBegin)),
                        RuleAddPriorityOption.ByPriorityNumber(i),
                        rulesSpecifications);
                }

                var deactiveDateBegin = CreateRandomDateBegin(currentYear);

                Add(CreateMultipleRule((ContentTypes)contentType, finalNumber, deactiveDateBegin, CreateRandomDateEnd(deactiveDateBegin), isActive: false),
                        RuleAddPriorityOption.ByPriorityNumber(finalNumber),
                        rulesSpecifications);
            }

            return rulesSpecifications;
        }

        private static RuleBuilderResult<ContentTypes, ConditionTypes> CreateMultipleRule(ContentTypes contentTypes, int value, DateTime dateBegin,
            DateTime? dateEnd, bool isActive = true) =>
                                     RuleBuilder
                             .NewRule<ContentTypes, ConditionTypes>()
                             .WithName($"Multi rule for test {contentTypes} {value}")
                             .WithContent(contentTypes, new { Value = value })
                             .WithDatesInterval(dateBegin, dateEnd)
                             .WithActive(isActive)
                             .WithCondition(cnb => cnb.AsComposed()
                                    .WithLogicalOperator(LogicalOperators.Or)
                                        .AddCondition(condition => condition
                                            .AsValued(ConditionTypes.RoyalNumber).OfDataType<int>()
                                            .WithComparisonOperator(Operators.Equal)
                                            .SetOperand(7)
                                            .Build())
                                        .AddCondition(condition => condition
                                            .AsValued(ConditionTypes.SumAll).OfDataType<IEnumerable<int>>()
                                            .WithComparisonOperator(Operators.In)
                                            .SetOperand(new int[] { 9, 8, 6 })
                                            .Build())
                                        .AddCondition(condition => condition.AsComposed()
                                            .WithLogicalOperator(LogicalOperators.And)
                                            .AddCondition(sub => sub
                                                .AsValued(ConditionTypes.IsPrimeNumber).OfDataType<bool>()
                                                .WithComparisonOperator(Operators.Equal)
                                                .SetOperand(false)
                                                .Build())
                                            .AddCondition(sub => sub
                                                .AsValued(ConditionTypes.SumAll).OfDataType<string>()
                                                .WithComparisonOperator(Operators.StartsWith)
                                                .SetOperand("15")
                                                .Build())
                                            .AddCondition(condition => condition.AsComposed()
                                                .WithLogicalOperator(LogicalOperators.And)
                                                .AddCondition(sub => sub
                                                    .AsValued(ConditionTypes.CanNumberBeDividedBy3).OfDataType<bool>()
                                                    .WithComparisonOperator(Operators.Equal)
                                                    .SetOperand(false)
                                                    .Build())
                                                .AddCondition(sub => sub
                                                    .AsValued(ConditionTypes.SumAll).OfDataType<string>()
                                                    .WithComparisonOperator(Operators.NotEqual)
                                                    .SetOperand(string.Empty)
                                                    .Build())
                                            .Build())
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
                                            .AddCondition(sub => sub
                                                .AsValued(ConditionTypes.CanNumberBeDividedBy3).OfDataType<bool>()
                                                .WithComparisonOperator(Operators.Equal)
                                                .SetOperand(false)
                                                .Build())
                                            .Build())
                                        .Build())
                            .Build();

        private void Add(
            RuleBuilderResult<ContentTypes, ConditionTypes> rule,
            RuleAddPriorityOption ruleAddPriorityOption, List<RuleSpecification> rulesSpecifications) => rulesSpecifications.Add(
                new RuleSpecification
                {
                    RuleBuilderResult = rule,
                    RuleAddPriorityOption = ruleAddPriorityOption,
                });

        private DateTime CreateRandomDateBegin(int year)
        {
            var months = random.Next(1, 11);
            year = random.Next(0, 1) + year;
            return new DateTime(year, 1, 1).AddMonths(months);
        }

        private DateTime? CreateRandomDateEnd(DateTime dateBegin)
        {
            var months = random.Next(0, 13);
            if (months == 13)
            {
                return null;
            }

            return dateBegin.AddMonths(months).AddDays(1);
        }
    }
}
