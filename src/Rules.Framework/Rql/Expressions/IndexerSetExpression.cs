namespace Rules.Framework.Rql.Expressions
{
    using Rules.Framework.Rql.Tokens;

    internal class IndexerSetExpression : Expression
    {
        public IndexerSetExpression(Expression instance, Token indexLeftDelimeter, Expression index, Token indexRightDelimeter, Token assign, Expression value)
            : base(instance.BeginPosition, value.EndPosition)
        {
            this.Instance = instance;
            this.IndexLeftDelimeter = indexLeftDelimeter;
            this.Index = index;
            this.IndexRightDelimeter = indexRightDelimeter;
            this.Assign = assign;
            this.Value = value;
        }

        public Token Assign { get; }

        public Expression Index { get; }

        public Token IndexLeftDelimeter { get; }

        public Token IndexRightDelimeter { get; }

        public Expression Instance { get; }

        public Expression Value { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitIndexerSetExpression(this);
    }
}