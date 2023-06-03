namespace Rules.Framework.Rql.Expressions
{
    using Rules.Framework.Rql.Tokens;

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