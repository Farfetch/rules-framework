namespace Rules.Framework.Rql.Pipeline.Parse.Strategies
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Tokens;

    internal class ObjectParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ObjectParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.IsMatchCurrentToken(TokenType.OBJECT))
            {
                var objectToken = parseContext.GetCurrentToken();
                if (parseContext.MoveNextIfNextToken(TokenType.BRACE_LEFT))
                {
                    _ = parseContext.MoveNext();
                    var objectAssignment = this.ParseObjectAssignment(parseContext);
                    if (parseContext.PanicMode)
                    {
                        return Expression.None;
                    }

                    var objectAssignments = new List<Expression> { objectAssignment };
                    while (parseContext.MoveNextIfNextToken(TokenType.COMMA))
                    {
                        _ = parseContext.MoveNext();
                        objectAssignment = this.ParseObjectAssignment(parseContext);
                        if (parseContext.PanicMode)
                        {
                            return Expression.None;
                        }

                        objectAssignments.Add(objectAssignment);
                    }

                    if (!parseContext.MoveNextIfNextToken(TokenType.BRACE_RIGHT))
                    {
                        parseContext.EnterPanicMode("Expected token '}'.", parseContext.GetCurrentToken());
                        return Expression.None;
                    }

                    return new NewObjectExpression(objectToken, objectAssignments.ToArray());
                }

                return new NewObjectExpression(objectToken, Array.Empty<Expression>());
            }

            return this.ParseExpressionWith<NothingParseStrategy>(parseContext);
        }

        private Expression ParseObjectAssignment(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(Constants.AllowedUnescapedIdentifierNames))
            {
                if (!parseContext.MoveNextIfCurrentToken(TokenType.ESCAPE) || !parseContext.IsMatchCurrentToken(Constants.AllowedEscapedIdentifierNames))
                {
                    parseContext.EnterPanicMode("Expected identifier for object property.", parseContext.GetCurrentToken());
                    return Expression.None;
                }
            }

            var left = parseContext.GetCurrentToken();

            if (!parseContext.MoveNextIfNextToken(TokenType.ASSIGN))
            {
                parseContext.EnterPanicMode("Expected token '='.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            var assign = parseContext.GetCurrentToken();
            _ = parseContext.MoveNext();
            var right = this.ParseExpressionWith<ArrayParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            return new AssignmentExpression(left, assign, right);
        }
    }
}