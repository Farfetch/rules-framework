namespace Rules.Framework.InMemory.Sample.Engine
{
    using System.Collections.Generic;
    using global::Rules.Framework.InMemory.Sample.Enums;

    internal interface IRuleSpecificationsRegistrar
    {
        RulesetNames[] Rulesets { get; }

        IEnumerable<RuleSpecification> GetRulesSpecifications();
    }
}