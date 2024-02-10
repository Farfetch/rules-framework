namespace Rules.Framework.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Tokens;

    [ExcludeFromCodeCoverage]
    internal class VariableDeclarationExpression : Expression
    {
        public VariableDeclarationExpression(Token keyword, Expression name)
            : base(keyword.BeginPosition, name.EndPosition)
        {
            this.Keyword = keyword;
            this.Name = name;
        }

        public Token Keyword { get; }

        public Expression Name { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitVariableDeclarationExpression(this);
    }
}