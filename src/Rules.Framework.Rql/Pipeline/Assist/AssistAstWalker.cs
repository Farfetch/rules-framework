namespace Rules.Framework.Rql.Pipeline.Assist
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Runtime.Types;
    using Rules.Framework.Rql.Tokens;

    internal class AssistAstWalker : IExpressionVisitor<Task<IAssistSuggestion[]>>, ISegmentVisitor<Task<IAssistSuggestion[]>>, IStatementVisitor<Task<IAssistSuggestion[]>>
    {
        private readonly RqlSourcePosition position;
        private readonly IRuntime runtime;
        private readonly IDictionary<string, object> storedContext;

        private AssistAstWalker(IRuntime runtime, RqlSourcePosition position)
        {
            this.runtime = runtime;
            this.position = position;
            this.storedContext = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public static AssistAstWalker Create(IRuntime runtime, RqlSourcePosition position)
            => new AssistAstWalker(runtime, position);

        public async Task<IAssistSuggestion[]> ProvideAssistSuggestionsAsync(IReadOnlyList<Statement> statements)
        {
            foreach (var statement in statements)
            {
                if (statement.ContainsPosition(position))
                {
                    var assistSuggestions = await statement.Accept(this).ConfigureAwait(false);
                    return assistSuggestions;
                }
            }

            return new IAssistSuggestion[0];
        }

        public Task<IAssistSuggestion[]> VisitAssignmentExpression(AssignmentExpression assignmentExpression) => Task.FromResult(EmptyAssistSuggestions());

        public Task<IAssistSuggestion[]> VisitBinaryExpression(BinaryExpression binaryExpression) => Task.FromResult(EmptyAssistSuggestions());

        public Task<IAssistSuggestion[]> VisitCardinalitySegment(CardinalitySegment cardinalitySegment)
        {
            if (cardinalitySegment.CardinalityKeyword == Expression.None)
            {
                return Task.FromResult(MultipleAssistSuggestions(nameof(TokenType.ALL), nameof(TokenType.ONE)));
            }

            if (cardinalitySegment.CardinalityKeyword is KeywordExpression keyword && cardinalitySegment.RuleKeyword is not KeywordExpression)
            {
                return keyword.Keyword.Type switch
                {
                    TokenType.ALL => Task.FromResult(MultipleAssistSuggestions(nameof(TokenType.RULES))),
                    TokenType.ONE => Task.FromResult(MultipleAssistSuggestions(nameof(TokenType.RULE))),
                    _ => Task.FromResult(EmptyAssistSuggestions()),
                };
            }

            return Task.FromResult(EmptyAssistSuggestions());
        }

        public Task<IAssistSuggestion[]> VisitDatesIntervalSegment(DatesIntervalSegment datesIntervalSegment)
        {
            if (datesIntervalSegment.SinceKeyword == Expression.None)
            {
                return Task.FromResult(MultipleAssistSuggestions(nameof(TokenType.SINCE)));
            }

            if (datesIntervalSegment.SinceDate is LiteralExpression sinceDateLiteral && sinceDateLiteral.Type == LiteralType.DateTime)
            {
                this.storedContext["Since-Date"] = sinceDateLiteral.Value;
            }

            if (datesIntervalSegment.UntilKeyword == Expression.None)
            {
                return Task.FromResult(MultipleAssistSuggestions(nameof(TokenType.UNTIL)));
            }

            if (datesIntervalSegment.SinceDate is LiteralExpression untilDateLiteral && untilDateLiteral.Type == LiteralType.DateTime)
            {
                this.storedContext["Until-Date"] = untilDateLiteral.Value;
            }

            return Task.FromResult(EmptyAssistSuggestions());
        }

        public async Task<IAssistSuggestion[]> VisitExpressionStatement(ExpressionStatement expressionStatement)
        {
            if (expressionStatement.Expression is not MatchExpression
                and not SearchExpression
                and not NewArrayExpression
                and not NewObjectExpression
                and not KeywordExpression)
            {
                return ExpressionSuggestions();
            }

            return await expressionStatement.Expression.Accept(this).ConfigureAwait(false);
        }

        public Task<IAssistSuggestion[]> VisitIdentifierExpression(IdentifierExpression identifierExpression) => Task.FromResult(EmptyAssistSuggestions());

        public async Task<IAssistSuggestion[]> VisitInputConditionSegment(InputConditionSegment inputConditionSegment)
        {
            if (inputConditionSegment.Left is PlaceholderExpression placeholderExpression)
            {
                return await placeholderExpression.Accept(this).ConfigureAwait(false);
            }

            if (inputConditionSegment.Operator == Token.None)
            {
                return MultipleAssistSuggestions(nameof(TokenType.IS));
            }

            return EmptyAssistSuggestions();
        }

        public async Task<IAssistSuggestion[]> VisitInputConditionsSegment(InputConditionsSegment inputConditionsSegment)
        {
            if (inputConditionsSegment.BeginToken == Token.None)
            {
                return EmptyAssistSuggestions();
            }

            foreach (var inputConditionSegment in inputConditionsSegment.InputConditions)
            {
                var assistSuggestions = await inputConditionSegment.Accept(this).ConfigureAwait(false);
                if (assistSuggestions.Length > 0)
                {
                    return assistSuggestions;
                }
            }

            return EmptyAssistSuggestions();
        }

        public Task<IAssistSuggestion[]> VisitKeywordExpression(KeywordExpression keywordExpression) => Task.FromResult(EmptyAssistSuggestions());

        public Task<IAssistSuggestion[]> VisitLiteralExpression(LiteralExpression literalExpression) => Task.FromResult(EmptyAssistSuggestions());

        public Task<IAssistSuggestion[]> VisitMatchDateSegment(MatchDateSegment matchDateSegment)
        {
            if (matchDateSegment.OnKeyword == Expression.None)
            {
                return Task.FromResult(MultipleAssistSuggestions(nameof(TokenType.ON)));
            }

            if (matchDateSegment.MatchDate is LiteralExpression literal && literal.Type == LiteralType.DateTime)
            {
                this.storedContext["Match-Date"] = literal.Value;
            }

            return Task.FromResult(EmptyAssistSuggestions());
        }

        public async Task<IAssistSuggestion[]> VisitMatchExpression(MatchExpression matchExpression)
        {
            if (matchExpression.Cardinality is not CardinalitySegment)
            {
                return MultipleAssistSuggestions(nameof(TokenType.ALL), nameof(TokenType.ONE));
            }

            var assistSuggestions = await matchExpression.Cardinality.Accept(this).ConfigureAwait(false);
            if (assistSuggestions.Length > 0)
            {
                return assistSuggestions;
            }

            assistSuggestions = await matchExpression.Ruleset.Accept(this).ConfigureAwait(false);
            if (assistSuggestions.Length > 0)
            {
                return assistSuggestions;
            }

            assistSuggestions = await matchExpression.MatchDate.Accept(this).ConfigureAwait(false);
            if (assistSuggestions.Length > 0)
            {
                return assistSuggestions;
            }

            if (matchExpression.InputConditions != Segment.None)
            {
                assistSuggestions = await matchExpression.InputConditions.Accept(this).ConfigureAwait(false);
                if (assistSuggestions.Length > 0)
                {
                    return assistSuggestions;
                }
            }
            else if (matchExpression.MatchDate is MatchDateSegment matchDateSegment && matchDateSegment.MatchDate is LiteralExpression literal && literal.Token.Next.Type != TokenType.SEMICOLON)
            {
                return MultipleAssistSuggestions(nameof(TokenType.WHEN));
            }

            return EmptyAssistSuggestions();
        }

        public Task<IAssistSuggestion[]> VisitNewArrayExpression(NewArrayExpression newArrayExpression)
        {
            if (newArrayExpression.InitializerBeginToken == Token.None)
            {
                return Task.FromResult(EmptyAssistSuggestions());
            }

            if (newArrayExpression.InitializerBeginToken.Type == TokenType.BRACE_LEFT)
            {
                if (newArrayExpression.Values.Length == 0)
                {
                    return Task.FromResult(EmptyAssistSuggestions());
                }

                var lastArrayValue = newArrayExpression.Values[newArrayExpression.Values.Length - 1];
                if (lastArrayValue == Expression.None || lastArrayValue is IdentifierExpression)
                {
                    return Task.FromResult(ExpressionSuggestions());
                }
            }

            return Task.FromResult(EmptyAssistSuggestions());
        }

        public Task<IAssistSuggestion[]> VisitNewObjectExpression(NewObjectExpression newObjectExpression) => Task.FromResult(EmptyAssistSuggestions());

        public Task<IAssistSuggestion[]> VisitNoneExpression(NoneExpression noneExpression) => Task.FromResult(EmptyAssistSuggestions());

        public Task<IAssistSuggestion[]> VisitNoneSegment(NoneSegment noneSegment) => Task.FromResult(EmptyAssistSuggestions());

        public Task<IAssistSuggestion[]> VisitNoneStatement(NoneStatement noneStatement) => Task.FromResult(EmptyAssistSuggestions());

        public Task<IAssistSuggestion[]> VisitOperatorSegment(OperatorSegment operatorSegment) => Task.FromResult(EmptyAssistSuggestions());

        public async Task<IAssistSuggestion[]> VisitPlaceholderExpression(PlaceholderExpression placeholderExpression)
        {
            if (placeholderExpression.ContainsPosition(this.position) && this.storedContext.TryGetValue("Ruleset-Name", out var rulesetName))
            {
                var dateBegin = DateTime.MinValue;
                var dateEnd = DateTime.MinValue;
                if (this.storedContext.TryGetValue("Match-Date", out var matchDate))
                {
                    dateBegin = ((DateTime)matchDate).AddDays(-1);
                    dateEnd = ((DateTime)matchDate).AddDays(1);
                }

                if (this.storedContext.TryGetValue("Since-Date", out var sinceDate) && this.storedContext.TryGetValue("Until-Date", out var untilDate))
                {
                    dateBegin = ((DateTime)sinceDate).AddDays(-1);
                    dateEnd = ((DateTime)untilDate).AddDays(1);
                }

                if (dateEnd != DateTime.MinValue)
                {
                    var conditions = await this.runtime.GetUniqueConditionsAsync((string)rulesetName, dateBegin, dateEnd).ConfigureAwait(false);
                    return MultipleAssistSuggestions(conditions.Value.Select(c => $"@{c.Unwrap<RqlString>().Value}").ToArray());
                }
            }

            return EmptyAssistSuggestions();
        }

        public async Task<IAssistSuggestion[]> VisitRulesetSegment(RulesetSegment rulesetSegment)
        {
            if (rulesetSegment.ForKeyword == Expression.None)
            {
                return MultipleAssistSuggestions(nameof(TokenType.FOR));
            }

            if (rulesetSegment.RulesetName == Expression.None || rulesetSegment.RulesetName.ContainsPosition(this.position))
            {
                var rulesets = await this.runtime.GetRulesetsAsync().ConfigureAwait(false);
                return MultipleAssistSuggestions(rulesets.Value.Select(r => @$"""{r.Unwrap<RqlRuleset>().Value.Name}""").ToArray());
            }
            else
            {
                var rulesetName = rulesetSegment.RulesetName as LiteralExpression;
                this.storedContext["Ruleset-Name"] = rulesetName.Value;
            }

            return EmptyAssistSuggestions();
        }

        public async Task<IAssistSuggestion[]> VisitSearchExpression(SearchExpression searchExpression)
        {
            if (searchExpression.RulesKeyword == Expression.None)
            {
                return MultipleAssistSuggestions(nameof(TokenType.RULES));
            }

            var assistSuggestions = await searchExpression.Ruleset.Accept(this).ConfigureAwait(false);
            if (assistSuggestions.Length > 0)
            {
                return assistSuggestions;
            }

            assistSuggestions = await searchExpression.DatesInterval.Accept(this).ConfigureAwait(false);
            if (assistSuggestions.Length > 0)
            {
                return assistSuggestions;
            }

            if (searchExpression.InputConditions != Segment.None)
            {
                assistSuggestions = await searchExpression.InputConditions.Accept(this).ConfigureAwait(false);
                if (assistSuggestions.Length > 0)
                {
                    return assistSuggestions;
                }
            }
            else if (searchExpression.DatesInterval is DatesIntervalSegment datesIntervalSegment && datesIntervalSegment.UntilDate is LiteralExpression literal && literal.Token.Next.Type != TokenType.SEMICOLON)
            {
                return MultipleAssistSuggestions(nameof(TokenType.WHEN));
            }

            return EmptyAssistSuggestions();
        }

        public async Task<IAssistSuggestion[]> VisitUnaryExpression(UnaryExpression expression)
            => await expression.Right.Accept(this).ConfigureAwait(false);

        private static IAssistSuggestion[] EmptyAssistSuggestions() => new IAssistSuggestion[0];

        private static IAssistSuggestion[] ExpressionSuggestions()
            => new[]
            {
                AssistSuggestion.New(nameof(TokenType.ARRAY)),
                AssistSuggestion.New(nameof(TokenType.MATCH)),
                AssistSuggestion.New(nameof(TokenType.NOTHING)),
                AssistSuggestion.New(nameof(TokenType.OBJECT)),
                AssistSuggestion.New(nameof(TokenType.SEARCH)),
            };

        private static IAssistSuggestion[] MultipleAssistSuggestions(params string[] lexemes)
        {
            var assistSuggestions = new IAssistSuggestion[lexemes.Length];
            for (var i = 0; i < lexemes.Length; i++)
            {
                assistSuggestions[i] = AssistSuggestion.New(lexemes[i]);
            }

            return assistSuggestions;
        }
    }
}