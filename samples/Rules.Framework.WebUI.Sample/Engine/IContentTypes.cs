namespace Rules.Framework.WebUI.Sample.Engine
{
    using System.Collections.Generic;

    internal interface IContentTypes
    {
        IEnumerable<RuleSpecification> GetRulesSpecifications();
    }
}