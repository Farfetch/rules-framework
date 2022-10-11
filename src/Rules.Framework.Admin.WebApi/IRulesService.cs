namespace Rules.Framework.Admin.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRulesService
    {
        Task<List<dynamic>> FindRulesAsync(string contentType, DateTime dateTime);

        IEnumerable<string> ListContents();
    }
}