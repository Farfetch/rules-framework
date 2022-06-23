namespace Rules.Framework.InMemory.Sample.Engine
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Rules.Framework.InMemory.Sample.Enums;
    using global::Rules.Framework.InMemory.Sample.Exceptions;

    internal class RulesBuilder
    {
        private readonly IEnumerable<IContentTypes> contentTypes;

        public RulesBuilder(IEnumerable<IContentTypes> contentTypes) => this.contentTypes = contentTypes;

        public async Task BuildAsync(RulesEngine<ContentTypes, ConditionTypes> rulesEngine)
        {
            foreach (var contentType in contentTypes)
            {
                var rulesSpecifications = contentType.GetRulesSpecifications();

                foreach (var ruleSpecification in rulesSpecifications)
                {
                    if (!ruleSpecification.RuleBuilderResult.IsSuccess)
                    {
                        throw new RulesBuilderException("Rules builder error getting rules specifications", ruleSpecification.RuleBuilderResult.Errors);
                    }

                    var ruleOperationResult = await rulesEngine
                        .AddRuleAsync(
                            ruleSpecification.RuleBuilderResult.Rule,
                            ruleSpecification.RuleAddPriorityOption
                        ).ConfigureAwait(false);

                    if (!ruleOperationResult.IsSuccess)
                    {
                        throw new RulesBuilderException("Rules builder error adding rules to engine", ruleOperationResult.Errors);
                    }
                }
            }
        }
    }
}