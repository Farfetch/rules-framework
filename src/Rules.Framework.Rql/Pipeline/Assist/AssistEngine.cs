namespace Rules.Framework.Rql.Pipeline.Assist
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Tokens;

    internal class AssistEngine : IAssistEngine
    {
        private static readonly Regex suggestionBeginCharRegex = new Regex("^[a-zA-Z@].*$");
        private readonly IRuntime runtime;

        public AssistEngine(IRuntime runtime)
        {
            this.runtime = runtime;
        }

        public async Task<IReadOnlyList<IAssistSuggestion>> ProcessAssistAsync(
            IReadOnlyList<Token> tokens,
            IReadOnlyList<Statement> statements,
            RqlSourcePosition position)
        {
            if (tokens.Count == 0)
            {
                return EmptyAssistSuggestions();
            }

            var token = FindTokenAtPosition(tokens, position);
            if (token == Token.None)
            {
                return EmptyAssistSuggestions();
            }

            if (!suggestionBeginCharRegex.IsMatch(token.UnescapedLexeme))
            {
                return EmptyAssistSuggestions();
            }

            var astAssistSuggestions = await AssistAstWalker.Create(this.runtime, position)
                .ProvideAssistSuggestionsAsync(statements).ConfigureAwait(false);

            if (astAssistSuggestions.Length > 0 && token != Token.None)
            {
                return astAssistSuggestions.Where(s => IsAssistSuggestionCompatibleWithToken(s, token))
                    .ToArray();
            }

            return astAssistSuggestions;
        }

        private static IReadOnlyList<IAssistSuggestion> EmptyAssistSuggestions() => new IAssistSuggestion[0];

        private static Token FindTokenAtPosition(IReadOnlyList<Token> tokens, RqlSourcePosition position)
        {
            foreach (var token in tokens)
            {
                if (token.BeginPosition <= position && token.EndPosition >= RqlSourcePosition.From(position.Line, position.Column - 1))
                {
                    return token;
                }
            }

            return Token.None;
        }

        private static bool IsAssistSuggestionCompatibleWithToken(IAssistSuggestion assistSuggestion, Token token)
        {
            return assistSuggestion.Lexeme.Replace("\"", "").StartsWith(token.Lexeme, StringComparison.OrdinalIgnoreCase);
        }
    }
}