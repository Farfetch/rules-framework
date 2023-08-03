namespace Rules.Framework.Rql.Ast.Statements
{
    using Rules.Framework.Rql.Ast.Expressions;

    internal class VariableBootstrapStatement : Statement
    {
        public VariableBootstrapStatement(Expression variableDeclaration, Expression assignable)
            : base(variableDeclaration.BeginPosition, assignable.EndPosition)
        {
            this.VariableDeclaration = variableDeclaration;
            this.Assignable = assignable;
        }

        public Expression Assignable { get; }

        public Expression VariableDeclaration { get; }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitVariableBootstrapStatement(this);
    }
}