namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Linq;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class RulesetParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        private static readonly LiteralType[] allowedLiteralTypesAsRuleset = new[] { LiteralType.Integer, LiteralType.String };

        private static readonly Lazy<string> allowedLiteralTypesMessage = new(() =>
            $"Only literals of types [{allowedLiteralTypesAsRuleset.Select(t => t.ToString()).Aggregate((t1, t2) => $"{t1}, {t2}")}] are allowed.");

        public RulesetParseStrategy(IParseStrategyProvider parseStrategyProvider) : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.FOR))
            {
                throw new InvalidOperationException("Unable to handle ruleset expression.");
            }

            if (!parseContext.MoveNext())
            {
                parseContext.EnterPanicMode("Expected ruleset name.", parseContext.GetNextToken());
                return Expression.None;
            }

            var rulesetNameExpression = this.ParseExpressionWith<BaseExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            if (rulesetNameExpression is LiteralExpression literalExpression && !allowedLiteralTypesAsRuleset.Contains(literalExpression.Type))
            {
                parseContext.EnterPanicMode($"Literal '{literalExpression.Token.Lexeme}' is not allowed as a valid ruleset. {allowedLiteralTypesMessage.Value}", literalExpression.Token);
                return Expression.None;
            }

            return rulesetNameExpression;
        }
    }
}