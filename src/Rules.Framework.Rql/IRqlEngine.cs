namespace Rules.Framework.Rql
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// The engine that processes Rule Query Language source.
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    public interface IRqlEngine : IDisposable
    {
        /// <summary>
        /// Executes the given Rule Query Language source.
        /// </summary>
        /// <param name="rql">The Rule Query Language source.</param>
        /// <returns>the result of the Rule Query Language source execution.</returns>
        Task<IEnumerable<IResult>> ExecuteAsync(string rql);

        /// <summary>
        /// Provides the assist suggestions for the writing of Rule Query Language. Takes in the
        /// source Rule Query Language and the position to get assist suggestions.
        /// </summary>
        /// <param name="rql">The Rule Query Language source.</param>
        /// <param name="position">
        /// The position, relative to the Rule Query Language source, to get assist suggestions.
        /// </param>
        /// <returns>a collection of assist suggestions.</returns>
        Task<IEnumerable<IAssistSuggestion>> ProvideAssistSuggestionsAsync(string rql, RqlSourcePosition position);
    }
}