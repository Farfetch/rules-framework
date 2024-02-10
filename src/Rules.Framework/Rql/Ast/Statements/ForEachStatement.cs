namespace Rules.Framework.Rql.Ast.Statements
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    [ExcludeFromCodeCoverage]
    internal class ForEachStatement : Statement
    {
        public ForEachStatement(Token forEachToken, Expression variableDeclaration, Token inToken, Expression sourceExpression, Statement forEachActionStatement)
            : base(forEachToken.BeginPosition, forEachActionStatement.EndPosition)
        {
            this.ForEachToken = forEachToken;
            this.VariableDeclaration = variableDeclaration;
            this.InToken = inToken;
            this.SourceExpression = sourceExpression;
            this.ForEachActionStatement = forEachActionStatement;
        }

        public Statement ForEachActionStatement { get; }

        public Token ForEachToken { get; }

        public Token InToken { get; }

        public Expression SourceExpression { get; }

        public Expression VariableDeclaration { get; }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitForEachStatement(this);
    }
}