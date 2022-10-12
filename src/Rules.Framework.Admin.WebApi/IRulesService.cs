namespace Rules.Framework.Admin.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    //TODO (we need to instaciate it for the client)
    public interface IRulesService
    {
        Task<List<dynamic>> FindRulesAsync(string contentType, DateTime dateTime);

        IEnumerable<string> ListContents();
    }
}