namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Globalization;
    using Rules.Framework.Rql.Expressions;

    internal class DateTimeLiteralParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public DateTimeLiteralParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var literalToken = parseContext.GetCurrentToken();
            if (!DateTime.TryParse((string)literalToken.Literal, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dateTimeLiteral))
            {
                parseContext.EnterPanicMode("Expected date token.", literalToken);
                return Expression.None;
            }

            return new LiteralExpression(LiteralType.DateTime, literalToken, dateTimeLiteral);
        }
    }
}