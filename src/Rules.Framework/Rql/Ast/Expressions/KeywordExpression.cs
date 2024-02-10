namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Tokens;

    [ExcludeFromCodeCoverage]
    internal class KeywordExpression : Expression
    {
        public KeywordExpression(Token keyword)
            : base(keyword.BeginPosition, keyword.EndPosition)
        {
            this.Keyword = keyword;
        }

        public Token Keyword { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitKeywordExpression(this);
    }
}