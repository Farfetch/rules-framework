namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Linq;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Tokens;

    internal class RulesetParseStrategy : ParseStrategyBase<Segment>, ISegmentParseStrategy
    {
        private static readonly LiteralType[] allowedLiteralTypesAsRuleset = new[] { LiteralType.Integer, LiteralType.String };

        private static readonly Lazy<string> allowedLiteralTypesMessage = new(() =>
            $"Only literals of types [{allowedLiteralTypesAsRuleset.Select(t => t.ToString()).Aggregate((t1, t2) => $"{t1}, {t2}")}] are allowed.");

        public RulesetParseStrategy(IParseStrategyProvider parseStrategyProvider) : base(parseStrategyProvider)
        {
        }

        public override Segment Parse(ParseContext parseContext)
        {
            var forKeyword = Expression.None;
            var rulesetName = Expression.None;
            if (!parseContext.MoveNextIfNextToken(TokenType.FOR))
            {
                parseContext.EnterPanicMode("Expected token 'FOR'.", parseContext.GetNextToken());
                return RulesetSegment.Create(forKeyword, rulesetName);
            }

            forKeyword = this.ParseExpressionWith<KeywordParseStrategy>(parseContext);
            if (!parseContext.MoveNext())
            {
                parseContext.EnterPanicMode("Expected ruleset name.", parseContext.GetNextToken());
                return RulesetSegment.Create(forKeyword, rulesetName);
            }

            rulesetName = this.ParseExpressionWith<BaseExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return RulesetSegment.Create(forKeyword, rulesetName);
            }

            if (rulesetName is LiteralExpression literalExpression && !allowedLiteralTypesAsRuleset.Contains(literalExpression.Type))
            {
                parseContext.EnterPanicMode($"Literal '{literalExpression.Token.Lexeme}' is not allowed as a valid ruleset. {allowedLiteralTypesMessage.Value}", literalExpression.Token);
            }

            return RulesetSegment.Create(forKeyword, rulesetName);
        }
    }
}