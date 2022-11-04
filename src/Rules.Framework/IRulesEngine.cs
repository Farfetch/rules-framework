namespace Rules.Framework
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Generic;

    /// <summary>
    /// Exposes rules engine logic to provide rule matches to requests.
    /// </summary>
    public interface IRulesEngine
    {
        /// <summary>
        /// Gets the content types.
        /// </summary>
        /// <returns>List of content types</returns>
        /// <exception cref="System.ArgumentException">
        /// Method only works if TContentType is a enum
        /// </exception>
        IEnumerable<ContentType> GetContentTypes();

        /// <summary>
        /// Gets the priority criterias.
        /// </summary>
        /// <returns>Rules engine priority criterias</returns>
        PriorityCriterias GetPriorityCriterias();

        /// <summary>
        /// Searches the asynchronous.
        /// </summary>
        /// <param name="genericSearchArgs">The search arguments.</param>
        /// <returns>List of generic rules</returns>
        Task<IEnumerable<GenericRule>> SearchAsync(SearchArgs<ContentType, ConditionType> genericSearchArgs);
    }
}