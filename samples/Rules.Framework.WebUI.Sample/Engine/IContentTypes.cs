namespace Rules.Framework.WebUI.Sample.Engine
{
    using System.Collections.Generic;
    using global::Rules.Framework.WebUI.Sample.Enums;

    internal interface IContentTypes
    {
        ContentTypes[] ContentTypes { get; }

        IEnumerable<RuleSpecification> GetRulesSpecifications();
    }
}