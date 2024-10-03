namespace Rules.Framework.WebUI.Sample.Engine
{
    using System.Collections.Generic;
    using global::Rules.Framework.WebUI.Sample.Enums;

    internal interface IRuleSpecificationsProvider
    {
        RulesetNames[] Rulesets { get; }

        IEnumerable<RuleSpecification> GetRulesSpecifications();
    }
}