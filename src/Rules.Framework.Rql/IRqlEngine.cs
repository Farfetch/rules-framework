namespace Rules.Framework.Rql
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRqlEngine : IDisposable
    {
        Task<IEnumerable<IResult>> ExecuteAsync(string rql);

        Task<IEnumerable<IAssistSuggestion>> ProvideAssistSuggestionsAsync(string rql, RqlSourcePosition position);
    }
}