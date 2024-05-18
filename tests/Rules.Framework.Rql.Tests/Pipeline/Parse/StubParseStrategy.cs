namespace Rules.Framework.Rql.Tests.Pipeline.Parse
{
    using System;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Pipeline.Parse;

    internal class StubParseStrategy : IExpressionParseStrategy, ISegmentParseStrategy, IStatementParseStrategy
    {
        public StubParseStrategy(IParseStrategyProvider parseStrategyProvider)
        {
            this.CreationDateTime = DateTime.UtcNow;
            this.ParseStrategyProvider = parseStrategyProvider;
        }

        public DateTime CreationDateTime { get; }
        public IParseStrategyProvider ParseStrategyProvider { get; }

        Expression IParseStrategy<Expression>.Parse(ParseContext parseContext) => throw new NotImplementedException("Implementation not needed for testing");

        Segment IParseStrategy<Segment>.Parse(ParseContext parseContext) => throw new NotImplementedException("Implementation not needed for testing");

        Statement IParseStrategy<Statement>.Parse(ParseContext parseContext) => throw new NotImplementedException("Implementation not needed for testing");
    }
}