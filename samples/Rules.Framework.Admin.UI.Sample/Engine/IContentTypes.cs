namespace Rules.Framework.Admin.UI.Sample.Engine
{
    using System.Collections.Generic;

    public interface IContentTypes
    {
        IEnumerable<RuleSpecification> GetRulesSpecifications();
    }
}