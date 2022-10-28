namespace Rules.Framework
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Generic;

    public interface IRulesEngine
    {
        IEnumerable<ContentType> GetContentTypes();

        PriorityCriterias GetPriorityCriterias();

        Task<IEnumerable<GenericRule>> SearchAsync(SearchArgs<ContentType, ConditionType> searchArgs);
    }
}