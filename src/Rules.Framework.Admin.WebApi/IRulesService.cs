namespace Rules.Framework.Admin.WebApi
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    //TODO (we need to create a instance for the client)
    public interface IRulesService
    {
        Task<IEnumerable<dynamic>> FindRulesAsync(string contentType);

        Task<string> GetRulePriorityOptionAsync();

        IEnumerable<string> ListContents();
    }
}