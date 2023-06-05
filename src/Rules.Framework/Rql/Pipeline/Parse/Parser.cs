namespace Rules.Framework.Rql.Pipeline.Parse
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Messages;
    using Rules.Framework.Rql.Statements;
    using Rules.Framework.Rql.Tokens;

    internal class Parser
    {
        public ParseResult Parse(IReadOnlyList<Token> tokens)
        {
            var parseContext = new ParseContext(tokens);
            var statements = new List<Statement>();

            using var messageContainer = new MessageContainer();
            while (parseContext.MoveNext())
            {
                var statement = HandleStatement(parseContext);
                if (parseContext.PanicMode)
                {
                    var panicModeInfo = parseContext.PanicModeInfo;
                    messageContainer.Error(
                        panicModeInfo.Message,
                        panicModeInfo.CauseToken.BeginPosition,
                        panicModeInfo.CauseToken.EndPosition);
                    Synchronize(parseContext);
                }
                else
                {
                    statements.Add(statement);
                }
            }

            var messages = messageContainer.Messages;
            if (messageContainer.ErrorsCount > 0)
            {
                return ParseResult.CreateError(messages);
            }

            return ParseResult.CreateSuccess(statements, messages);
        }

        private static Expression HandleContentType(ParseContext parseContext)
        {
            if (parseContext.MoveNext() && parseContext.MoveNextConditionally(TokenType.FOR))
            {
                if (parseContext.IsMatch(TokenType.IDENTIFIER) || parseContext.IsMatch(TokenType.STRING))
                {
                    var contentType = HandleLiteral(parseContext, LiteralType.String);
                    if (parseContext.PanicMode)
                    {
                        return Expression.None;
                    }

                    return contentType;
                }

                parseContext.EnterPanicMode("Expect content type name.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            parseContext.EnterPanicMode("Expect token 'FOR'.", parseContext.GetCurrentToken());
            return Expression.None;
        }

        private static KeywordExpression HandleKeyword(ParseContext parseContext)
        {
            var keywordToken = parseContext.GetCurrentToken();
            return new KeywordExpression(keywordToken);
        }

        private static Expression HandleLiteral(ParseContext parseContext, LiteralType literalType)
        {
            var literalToken = parseContext.GetCurrentToken();
            object literalValue;
            switch (literalType)
            {
                case LiteralType.String:
                case LiteralType.Integer:
                case LiteralType.Decimal:
                case LiteralType.Bool:
                    literalValue = literalToken.Literal;
                    break;

                case LiteralType.DateTime:
                    if (!DateTime.TryParse((string)literalToken.Literal, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dateTimeLiteral))
                    {
                        parseContext.EnterPanicMode("Expected date token.", literalToken);
                        return Expression.None;
                    }

                    literalValue = dateTimeLiteral;
                    break;

                case LiteralType.None:
                default:
                    throw new NotSupportedException($"The literal type '{literalType}' is not supported.");
            }
            return new LiteralExpression(literalType, literalToken, literalValue);
        }

        private static Expression HandleMatch(ParseContext parseContext)
        {
            var cardinality = HandleMatchCardinality(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            var contentType = HandleContentType(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            var matchDate = HandleMatchDate(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            var inputConditionExpressions = HandlQueryInputConditions(parseContext);
            if (parseContext.PanicMode)
            {
                return Expression.None;
            }

            return new MatchExpression(cardinality, contentType, matchDate, inputConditionExpressions);
        }

        private static Expression HandleMatchCardinality(ParseContext parseContext)
        {
            if (parseContext.MoveNext())
            {
                if (parseContext.IsMatch(TokenType.ONE))
                {
                    var oneCardinalityKeyword = HandleKeyword(parseContext);
                    if (parseContext.MoveNext() && !parseContext.IsMatch(TokenType.RULE))
                    {
                        parseContext.EnterPanicMode("Expect token 'RULE'.", parseContext.GetCurrentToken());
                        return Expression.None;
                    }

                    var ruleKeyword = HandleKeyword(parseContext);

                    return new CardinalityExpression(oneCardinalityKeyword, ruleKeyword);
                }

                if (parseContext.IsMatch(TokenType.ALL))
                {
                    var allCardinalityKeyword = HandleKeyword(parseContext);
                    if (parseContext.MoveNext() && !parseContext.IsMatch(TokenType.RULES))
                    {
                        parseContext.EnterPanicMode("Expect token 'RULES'.", parseContext.GetCurrentToken());
                        return Expression.None;
                    }

                    var ruleKeyword = HandleKeyword(parseContext);

                    return new CardinalityExpression(allCardinalityKeyword, ruleKeyword);
                }
            }

            parseContext.EnterPanicMode("Expect tokens 'ONE' or 'ALL'.", parseContext.GetCurrentToken());
            return Expression.None;
        }

        private static Expression HandleMatchDate(ParseContext parseContext)
        {
            if (parseContext.MoveNext() && parseContext.MoveNextConditionally(TokenType.ON))
            {
                if (parseContext.IsMatch(TokenType.STRING))
                {
                    var matchDate = HandleLiteral(parseContext, LiteralType.DateTime);
                    if (parseContext.PanicMode)
                    {
                        return Expression.None;
                    }

                    return matchDate;
                }

                parseContext.EnterPanicMode("Expect match date and time.", parseContext.GetCurrentToken());
                return Expression.None;
            }

            parseContext.EnterPanicMode("Expect token 'ON'.", parseContext.GetCurrentToken());
            return Expression.None;
        }

        private static Expression HandleQueryInputCondition(ParseContext parseContext)
        {
            if (parseContext.IsMatch(TokenType.IDENTIFIER) || parseContext.IsMatch(TokenType.STRING))
            {
                var leftLiteralToken = parseContext.GetCurrentToken();
                var leftExpression = new LiteralExpression(LiteralType.String, leftLiteralToken, leftLiteralToken.Literal);

                if (!parseContext.MoveNext() || !parseContext.IsMatch(TokenType.IS))
                {
                    parseContext.EnterPanicMode("Expect token 'IS'.", parseContext.GetCurrentToken());
                    return Expression.None;
                }

                var operatorToken = parseContext.GetCurrentToken();

                if (parseContext.MoveNext() && parseContext.IsMatch(TokenType.STRING, TokenType.INT, TokenType.DECIMAL, TokenType.BOOL))
                {
                    var literalToken = parseContext.GetCurrentToken();
                    var literalType = literalToken.Type switch
                    {
                        TokenType.STRING => LiteralType.String,
                        TokenType.INT => LiteralType.Integer,
                        TokenType.DECIMAL => LiteralType.Decimal,
                        TokenType.BOOL => LiteralType.Bool,
                        _ => throw new NotSupportedException($"Given token type '{literalToken.Type}' is not a supported literal type."),
                    };
                    var rightExpression = HandleLiteral(parseContext, literalType);
                    if (parseContext.PanicMode)
                    {
                        return Expression.None;
                    }

                    return new InputConditionExpression(leftExpression, operatorToken, rightExpression);
                }

                parseContext.EnterPanicMode("Expect literal for condition", parseContext.GetCurrentToken());
                return Expression.None;
            }

            parseContext.EnterPanicMode("Expect identifier for condition", parseContext.GetCurrentToken());
            return Expression.None;
        }

        private static Expression HandleSearch(ParseContext parseContext)
        {
            if (parseContext.MoveNext() && parseContext.IsMatch(TokenType.RULES))
            {
                var contentType = HandleContentType(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                (var dateBegin, var dateEnd) = HandleSearchDateRange(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                var inputConditionExpressions = HandlQueryInputConditions(parseContext);
                if (parseContext.PanicMode)
                {
                    return Expression.None;
                }

                return new SearchExpression(contentType, dateBegin, dateEnd, inputConditionExpressions);
            }

            parseContext.EnterPanicMode("Expect token 'RULES'.", parseContext.GetCurrentToken());
            return Expression.None;
        }

        private static (Expression, Expression) HandleSearchDateRange(ParseContext parseContext)
        {
            if (parseContext.MoveNext() && parseContext.MoveNextConditionally(TokenType.ON))
            {
                if (parseContext.IsMatch(TokenType.STRING))
                {
                    var dateBegin = HandleLiteral(parseContext, LiteralType.DateTime);
                    if (parseContext.PanicMode)
                    {
                        return (Expression.None, Expression.None);
                    }

                    if (parseContext.MoveNext() && parseContext.MoveNextConditionally(TokenType.TO))
                    {
                        if (parseContext.IsMatch(TokenType.STRING))
                        {
                            var dateEnd = HandleLiteral(parseContext, LiteralType.DateTime);
                            if (parseContext.PanicMode)
                            {
                                return (Expression.None, Expression.None);
                            }

                            return (dateBegin, dateEnd);
                        }

                        parseContext.EnterPanicMode("Expect end date and time.", parseContext.GetCurrentToken());
                        return (Expression.None, Expression.None);
                    }

                    parseContext.EnterPanicMode("Expect token 'TO'.", parseContext.GetCurrentToken());
                    return (Expression.None, Expression.None);
                }

                parseContext.EnterPanicMode("Expect begin date and time.", parseContext.GetCurrentToken());
                return (Expression.None, Expression.None);
            }

            parseContext.EnterPanicMode("Expect token 'ON'.", parseContext.GetCurrentToken());
            return (Expression.None, Expression.None);
        }

        private static Statement HandleStatement(ParseContext parseContext)
        {
            if (parseContext.IsMatch(TokenType.MATCH))
            {
                var matchExpression = HandleMatch(parseContext);
                if (parseContext.PanicMode)
                {
                    return Statement.None;
                }

                return new QueryStatement(matchExpression);
            }

            if (parseContext.IsMatch(TokenType.SEARCH))
            {
                var searchExpression = HandleSearch(parseContext);
                if (parseContext.PanicMode)
                {
                    return Statement.None;
                }

                return new QueryStatement(searchExpression);
            }

            _ = parseContext.MoveNext();
            parseContext.EnterPanicMode("Expected statement begin (MATCH, SEARCH).", parseContext.GetCurrentToken());
            return Statement.None;
        }

        private static IEnumerable<Expression> HandlQueryInputConditions(ParseContext parseContext)
        {
            if (parseContext.MoveNext() && parseContext.MoveNextConditionally(TokenType.WITH))
            {
                if (!parseContext.MoveNextConditionally(TokenType.BRACE_LEFT))
                {
                    parseContext.EnterPanicMode("Expect '{' after WITH.", parseContext.GetCurrentToken());
                    return Enumerable.Empty<Expression>();
                }

                var inputConditionExpressions = new List<Expression>();
                while (true)
                {
                    var inputConditionExpression = HandleQueryInputCondition(parseContext);
                    if (parseContext.PanicMode)
                    {
                        return Enumerable.Empty<Expression>();
                    }

                    inputConditionExpressions.Add(inputConditionExpression);

                    if (parseContext.MoveNext())
                    {
                        if (parseContext.IsMatch(TokenType.COMMA))
                        {
                            _ = parseContext.MoveNext();
                            continue;
                        }

                        if (parseContext.IsMatch(TokenType.BRACE_RIGHT))
                        {
                            _ = parseContext.MoveNext();
                            break;
                        }
                    }

                    parseContext.EnterPanicMode("Expect ',' or '}' after input condition.", parseContext.GetCurrentToken());
                    return Enumerable.Empty<Expression>();
                }

                return inputConditionExpressions;
            }

            return Enumerable.Empty<Expression>();
        }

        private static void Synchronize(ParseContext parseContext)
        {
            while (parseContext.MoveNext())
            {
                switch (parseContext.GetCurrentToken().Type)
                {
                    case TokenType.SEMICOLON:
                    case TokenType.EOF:
                        return;

                    default:
                        break;
                }
            }
        }
    }
}