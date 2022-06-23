namespace Rules.Framework.InMemory.Sample.Engine
{
    using System.Collections.Generic;

    internal interface IContentTypes
    {
        IEnumerable<RuleSpecification> GetRulesSpecifications();
    }
}