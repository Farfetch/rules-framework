namespace Rules.Framework.Rql.Pipeline.Assist
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Tokens;

    internal interface IAssistEngine
    {
        Task<IReadOnlyList<IAssistSuggestion>> ProcessAssistAsync(
            IReadOnlyList<Token> tokens,
            IReadOnlyList<Statement> statements,
            RqlSourcePosition position);
    }
}