namespace Rules.Framework.Rql.Ast.Segments
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Ast.Expressions;

    [ExcludeFromCodeCoverage]
    internal class RulesetSegment : Segment
    {
        public RulesetSegment(Expression forKeyword, Expression rulesetName)
            : base(forKeyword.BeginPosition, rulesetName.EndPosition)
        {
            this.ForKeyword = forKeyword;
            this.RulesetName = rulesetName;
        }

        public Expression ForKeyword { get; }

        public Expression RulesetName { get; }

        public static RulesetSegment Create(Expression forKeyword, Expression rulesetName)
            => new RulesetSegment(forKeyword, rulesetName);

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitRulesetSegment(this);
    }
}