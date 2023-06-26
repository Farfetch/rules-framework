namespace Rules.Framework.Rql.Expressions
{
    using Rules.Framework.Rql.Tokens;

    internal class IndexerExpression : Expression
    {
        public IndexerExpression(Expression instance, Token indexLeftDelimeter, Expression index, Token indexRightDelimeter)
            : base(instance.BeginPosition, indexRightDelimeter.EndPosition)
        {
            this.Instance = instance;
            this.IndexLeftDelimeter = indexLeftDelimeter;
            this.Index = index;
            this.IndexRightDelimeter = indexRightDelimeter;
        }

        public Expression Index { get; }

        public Token IndexLeftDelimeter { get; }

        public Token IndexRightDelimeter { get; }

        public Expression Instance { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitIndexerExpression(this);
    }
}