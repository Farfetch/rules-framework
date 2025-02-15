namespace Rules.Framework.Rql.Ast.Segments
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Tokens;

    [ExcludeFromCodeCoverage]
    internal class InputConditionsSegment : Segment
    {
        public InputConditionsSegment(Expression whenKeyword, Token beginToken, Segment[] inputConditions, Token endToken)
            : base(whenKeyword.BeginPosition, endToken.EndPosition)
        {
            this.WhenKeyword = whenKeyword;
            this.BeginToken = beginToken;
            this.InputConditions = inputConditions;
            this.EndToken = endToken;
        }

        public Token BeginToken { get; }

        public Token EndToken { get; }

        public Segment[] InputConditions { get; }

        public Expression WhenKeyword { get; }

        public static InputConditionsSegment Create(Expression whenKeyword, Token beginToken, Segment[] inputConditions, Token endToken)
            => new InputConditionsSegment(whenKeyword, beginToken, inputConditions, endToken);

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitInputConditionsSegment(this);
    }
}