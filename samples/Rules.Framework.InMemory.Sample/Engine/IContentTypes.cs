namespace Rules.Framework.InMemory.Sample.Engine
{
    using System.Collections.Generic;
    using global::Rules.Framework.InMemory.Sample.Enums;

    internal interface IContentTypes
    {
        ContentTypes ContentType { get; }

        IEnumerable<RuleSpecification> GetRulesSpecifications();
    }
}