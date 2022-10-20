namespace Rules.Framework.WebApi
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    //TODO (we need to create a instance for the client)
    public interface IRulesService
    {
        /// <summary>
        /// Finds the rules asynchronous.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>List rules</returns>
        Task<IEnumerable<dynamic>> FindRulesAsync(string contentType);

        /// <summary>
        /// Gets the rule priority option asynchronous.
        /// </summary>
        /// <returns>Rule priority option</returns>
        Task<string> GetRulePriorityOptionAsync();

        /// <summary>
        /// Lists the contents.
        /// </summary>
        /// <returns>List of contents</returns>
        IEnumerable<string> ListContents();
    }
}