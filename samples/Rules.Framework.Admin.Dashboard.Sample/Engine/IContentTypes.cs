namespace Rules.Framework.Admin.Dashboard.Sample.Engine
{
    using System.Collections.Generic;

    public interface IContentTypes
    {
        IEnumerable<RuleSpecification> GetRulesSpecifications();
    }
}