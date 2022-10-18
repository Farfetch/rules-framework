namespace Rules.Framework.UI.Sample
{
    using System.Collections.Generic;

    public interface IContentTypes
    {
        IEnumerable<RuleSpecification> GetRulesSpecifications();
    }
}