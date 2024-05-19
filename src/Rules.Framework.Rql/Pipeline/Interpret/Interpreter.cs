namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Rql.Ast;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Ast.Statements;
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Runtime.RuleManipulation;
    using Rules.Framework.Rql.Runtime.Types;
    using Rules.Framework.Rql.Tokens;

    internal class Interpreter<TContentType, TConditionType> : IInterpreter, IExpressionVisitor<Task<IRuntimeValue>>, ISegmentVisitor<Task<object>>, IStatementVisitor<Task<IResult>>
    {
        private readonly IReverseRqlBuilder reverseRqlBuilder;
        private bool disposedValue;
        private IRuntime<TContentType, TConditionType> runtime;

        public Interpreter(
            IRuntime<TContentType, TConditionType> runtime,
            IReverseRqlBuilder reverseRqlBuilder)
        {
            this.runtime = runtime;
            this.reverseRqlBuilder = reverseRqlBuilder;
        }

        public async Task<InterpretResult> InterpretAsync(IReadOnlyList<Statement> statements)
        {
            var interpretResult = new InterpretResult();
            foreach (var statement in statements)
            {
                try
                {
                    var statementResult = await statement.Accept(this).ConfigureAwait(false);
                    interpretResult.AddStatementResult(statementResult);
                }
                catch (InterpreterException ie)
                {
                    var errorStatementResult = new ErrorStatementResult(ie.Message, ie.Rql, ie.BeginPosition, ie.EndPosition);
                    interpretResult.AddStatementResult(errorStatementResult);
                    break;
                }
            }

            return interpretResult;
        }

        public Task<IRuntimeValue> VisitAssignmentExpression(AssignmentExpression assignmentExpression)
        {
            throw new NotImplementedException("To be supported on future release.");
        }

        public async Task<IRuntimeValue> VisitBinaryExpression(BinaryExpression binaryExpression)
        {
            try
            {
                var left = await binaryExpression.LeftExpression.Accept(this).ConfigureAwait(false);
                var right = await binaryExpression.RightExpression.Accept(this).ConfigureAwait(false);
                var rqlOperator = (RqlOperators)await binaryExpression.OperatorSegment.Accept(this).ConfigureAwait(false);
                return this.runtime.ApplyBinary(left, rqlOperator, right);
            }
            catch (RuntimeException ex)
            {
                throw CreateInterpreterException(ex.Message, binaryExpression);
            }
        }

        public async Task<object> VisitCardinalitySegment(CardinalitySegment expression) => await expression.CardinalityKeyword.Accept(this).ConfigureAwait(false);

        public async Task<IResult> VisitExpressionStatement(ExpressionStatement expressionStatement)
        {
            var rql = this.reverseRqlBuilder.BuildRql(expressionStatement);
            var expressionResult = await expressionStatement.Expression.Accept(this).ConfigureAwait(false);
            return new ExpressionStatementResult(rql, expressionResult);
        }

        public Task<IRuntimeValue> VisitIdentifierExpression(IdentifierExpression identifierExpression)
            => Task.FromResult<IRuntimeValue>(new RqlString(identifierExpression.Identifier.UnescapedLexeme));

        public async Task<object> VisitInputConditionSegment(InputConditionSegment inputConditionExpression)
        {
            var conditionType = await this.HandleConditionTypeAsync(inputConditionExpression.Left).ConfigureAwait(false);
            var conditionValue = await inputConditionExpression.Right.Accept(this).ConfigureAwait(false);
            return new Condition<TConditionType>(conditionType, conditionValue.RuntimeValue);
        }

        public async Task<object> VisitInputConditionsSegment(InputConditionsSegment inputConditionsExpression)
        {
            var inputConditions = inputConditionsExpression.InputConditions;
            var inputConditionsLength = inputConditions.Length;
            var conditions = new Condition<TConditionType>[inputConditionsLength];
            for (int i = 0; i < inputConditionsLength; i++)
            {
                conditions[i] = (Condition<TConditionType>)await inputConditions[i].Accept(this).ConfigureAwait(false);
            }

            return conditions;
        }

        public Task<IRuntimeValue> VisitKeywordExpression(KeywordExpression keywordExpression)
            => Task.FromResult<IRuntimeValue>(new RqlString(keywordExpression.Keyword.Lexeme));

        public Task<IRuntimeValue> VisitLiteralExpression(LiteralExpression literalExpression)
        {
            return Task.FromResult<IRuntimeValue>(literalExpression.Type switch
            {
                LiteralType.Bool when literalExpression.Value is null => new RqlNothing(),
                LiteralType.Bool => new RqlBool((bool)literalExpression.Value),
                LiteralType.Decimal when literalExpression.Value is null => new RqlNothing(),
                LiteralType.Decimal => new RqlDecimal((decimal)literalExpression.Value),
                LiteralType.Integer when literalExpression.Value is null => new RqlNothing(),
                LiteralType.Integer => new RqlInteger((int)literalExpression.Value),
                LiteralType.String when literalExpression.Value is null => new RqlNothing(),
                LiteralType.String => new RqlString((string)literalExpression.Value),
                LiteralType.DateTime when literalExpression.Value is null => new RqlNothing(),
                LiteralType.DateTime => new RqlDate(((DateTime)literalExpression.Value).ToUniversalTime()),
                LiteralType.Undefined => new RqlNothing(),
                _ when literalExpression.Value is null => new RqlNothing(),
                _ => throw new NotSupportedException($"Literal with type '{literalExpression.Type}' is not supported."),
            });
        }

        public async Task<IRuntimeValue> VisitMatchExpression(MatchExpression matchExpression)
        {
            try
            {
                var cardinality = (RqlString)await matchExpression.Cardinality.Accept(this).ConfigureAwait(false);
                var contentType = await this.HandleContentTypeAsync(matchExpression.ContentType).ConfigureAwait(false);
                var matchDate = (RqlDate)await matchExpression.MatchDate.Accept(this).ConfigureAwait(false);
                var inputConditions = await matchExpression.InputConditions.Accept(this).ConfigureAwait(false);
                var conditions = inputConditions is null ? Array.Empty<Condition<TConditionType>>() : (IEnumerable<Condition<TConditionType>>)inputConditions;
                var matchCardinality = string.Equals(cardinality.Value, "ONE", StringComparison.OrdinalIgnoreCase)
                    ? MatchCardinality.One
                    : MatchCardinality.All;
                var matchRulesArgs = new MatchRulesArgs<TContentType, TConditionType>
                {
                    Conditions = conditions,
                    ContentType = contentType,
                    MatchCardinality = matchCardinality,
                    MatchDate = matchDate,
                };

                return await this.runtime.MatchRulesAsync(matchRulesArgs).ConfigureAwait(false);
            }
            catch (RuntimeException ex)
            {
                throw CreateInterpreterException(ex.Message, matchExpression);
            }
        }

        public async Task<IRuntimeValue> VisitNewArrayExpression(NewArrayExpression newArrayExpression)
        {
            var sizeValue = await newArrayExpression.Size.Accept(this).ConfigureAwait(false);
            var size = sizeValue is RqlInteger integer ? integer.Value : newArrayExpression.Values.Length;
            var hasArrayInitializer = newArrayExpression.Values.Length > 0;
            var rqlArray = new RqlArray(size, !hasArrayInitializer);

            if (hasArrayInitializer)
            {
                for (var i = 0; i < size; i++)
                {
                    var value = await newArrayExpression.Values[i].Accept(this).ConfigureAwait(false);
                    rqlArray.Value[i] = new RqlAny(value);
                }
            }

            return rqlArray;
        }

        public async Task<IRuntimeValue> VisitNewObjectExpression(NewObjectExpression newObjectExpression)
        {
            var rqlObject = new RqlObject();
            var propertyAssignments = newObjectExpression.PropertyAssignments;
            for (int i = 0; i < propertyAssignments.Length; i++)
            {
                var assignment = (AssignmentExpression)propertyAssignments[i];
                var left = (RqlString)await assignment.Left.Accept(this).ConfigureAwait(false);
                var right = await assignment.Right.Accept(this).ConfigureAwait(false);
                rqlObject.SetPropertyValue(left, new RqlAny(right));
            }

            return rqlObject;
        }

        public Task<IRuntimeValue> VisitNoneExpression(NoneExpression noneExpression) => Task.FromResult<IRuntimeValue>(new RqlNothing());

        public Task<object> VisitNoneSegment(NoneSegment noneSegment) => Task.FromResult<object>(null!);

        public Task<IResult> VisitNoneStatement(NoneStatement statement) => Task.FromResult<IResult>(new ExpressionStatementResult(string.Empty, new RqlNothing()));

        public Task<object> VisitOperatorSegment(OperatorSegment operatorExpression)
        {
            var resultOperator = RqlOperators.None;
            switch (operatorExpression.Tokens[0].Type)
            {
                case TokenType.AND:
                    resultOperator = RqlOperators.And;
                    break;

                case TokenType.ASSIGN:
                    resultOperator = RqlOperators.Assign;
                    break;

                case TokenType.EQUAL:
                    resultOperator = RqlOperators.Equals;
                    break;

                case TokenType.GREATER_THAN:
                    resultOperator = RqlOperators.GreaterThan;
                    break;

                case TokenType.GREATER_THAN_OR_EQUAL:
                    resultOperator = RqlOperators.GreaterThanOrEquals;
                    break;

                case TokenType.IN:
                    resultOperator = RqlOperators.In;
                    break;

                case TokenType.LESS_THAN:
                    resultOperator = RqlOperators.LesserThan;
                    break;

                case TokenType.LESS_THAN_OR_EQUAL:
                    resultOperator = RqlOperators.LesserThanOrEquals;
                    break;

                case TokenType.MINUS:
                    resultOperator = RqlOperators.Minus;
                    break;

                case TokenType.NOT:
                    if (operatorExpression.Tokens.Length > 1 && operatorExpression.Tokens[1].Type == TokenType.IN)
                    {
                        resultOperator = RqlOperators.NotIn;
                    }
                    break;

                case TokenType.NOT_EQUAL:
                    resultOperator = RqlOperators.NotEquals;
                    break;

                case TokenType.OR:
                    resultOperator = RqlOperators.Or;
                    break;

                case TokenType.PLUS:
                    resultOperator = RqlOperators.Plus;
                    break;

                case TokenType.SLASH:
                    resultOperator = RqlOperators.Slash;
                    break;

                case TokenType.STAR:
                    resultOperator = RqlOperators.Star;
                    break;
            }

            if (resultOperator == RqlOperators.None)
            {
                var tokenTypes = operatorExpression.Tokens.Select(t => $"'{t.Type}'").Aggregate((t1, t2) => $"{t1}, {t2}");
                throw new NotSupportedException($"The tokens with types [{tokenTypes}] are not supported as a valid operator.");
            }

            return Task.FromResult<object>(resultOperator);
        }

        public Task<IRuntimeValue> VisitPlaceholderExpression(PlaceholderExpression placeholderExpression)
            => Task.FromResult<IRuntimeValue>(new RqlString((string)placeholderExpression.Token.Literal));

        public async Task<IRuntimeValue> VisitSearchExpression(SearchExpression searchExpression)
        {
            try
            {
                var contentType = await this.HandleContentTypeAsync(searchExpression.ContentType).ConfigureAwait(false);
                var dateBegin = (RqlDate)await searchExpression.DateBegin.Accept(this).ConfigureAwait(false);
                var dateEnd = (RqlDate)await searchExpression.DateEnd.Accept(this).ConfigureAwait(false);
                var conditions = (IEnumerable<Condition<TConditionType>>)await searchExpression.InputConditions.Accept(this).ConfigureAwait(false);
                var searchRulesArgs = new SearchRulesArgs<TContentType, TConditionType>
                {
                    Conditions = conditions ?? Enumerable.Empty<Condition<TConditionType>>(),
                    ContentType = contentType,
                    DateBegin = dateBegin,
                    DateEnd = dateEnd,
                };

                return await this.runtime.SearchRulesAsync(searchRulesArgs).ConfigureAwait(false);
            }
            catch (RuntimeException ex)
            {
                throw CreateInterpreterException(ex.Message, searchExpression);
            }
        }

        public async Task<IRuntimeValue> VisitUnaryExpression(UnaryExpression unaryExpression)
        {
            try
            {
                var @operator = unaryExpression.Operator.Lexeme switch
                {
                    "-" => RqlOperators.Minus,
                    _ => RqlOperators.None,
                };
                var right = await unaryExpression.Right.Accept(this).ConfigureAwait(false);
                return this.runtime.ApplyUnary(right, @operator);
            }
            catch (RuntimeException re)
            {
                throw CreateInterpreterException(re.Errors, unaryExpression);
            }
        }

        private Exception CreateInterpreterException(IEnumerable<string> errors, IAstElement astElement)
        {
            var rql = this.reverseRqlBuilder.BuildRql(astElement);
            var separator = $"{Environment.NewLine}\t - ";
            var errorsText = string.Join(separator, errors);
            return new InterpreterException(
                $"Errors have occurred while executing sentence:{separator}{errorsText}",
                rql,
                astElement.BeginPosition,
                astElement.EndPosition);
        }

        private Exception CreateInterpreterException(string error, IAstElement astElement)
        {
            return CreateInterpreterException(new[] { error }, astElement);
        }

        private async Task<TConditionType> HandleConditionTypeAsync(Expression conditionTypeExpression)
        {
            var conditionTypeName = (RqlString)await conditionTypeExpression.Accept(this).ConfigureAwait(false);
            object conditionType;
            var type = typeof(TConditionType);

            if (type == typeof(string))
            {
                conditionType = conditionTypeName.Value;
            }
            else
            {
#if NETSTANDARD2_0
                try
                {
                    conditionType = Enum.Parse(type, conditionTypeName.Value);
                }
                catch (Exception)
                {
                    throw CreateInterpreterException(new[] { FormattableString.Invariant($"Condition type of name '{conditionTypeName}' was not found.") }, conditionTypeExpression);
                }
#else
                if (!Enum.TryParse(type, conditionTypeName.Value, out conditionType))
                {
                    throw CreateInterpreterException(new[] { FormattableString.Invariant($"Condition type of name '{conditionTypeName}' was not found.") }, conditionTypeExpression);
                }
#endif
            }

            return (TConditionType)conditionType;
        }

        private async Task<TContentType> HandleContentTypeAsync(Expression contentTypeExpression)
        {
            var rawValue = await contentTypeExpression.Accept(this).ConfigureAwait(false);
            var value = RqlTypes.Any.IsAssignableTo(rawValue.Type) ? ((RqlAny)rawValue).Unwrap() : rawValue;
            if (!RqlTypes.String.IsAssignableTo(value.Type))
            {
                throw CreateInterpreterException($"Expected a content type value of type '{RqlTypes.String.Name}' but found '{value.Type.Name}' instead", contentTypeExpression);
            }

            try
            {
                var type = typeof(TContentType);
                if (type == typeof(string))
                {
                    return (TContentType)((RqlString)value).RuntimeValue;
                }

                return (TContentType)Enum.Parse(type, ((RqlString)value).Value, ignoreCase: true);
            }
            catch (Exception)
            {
                throw CreateInterpreterException($"The content type value '{value.RuntimeValue}' was not found", contentTypeExpression);
            }
        }
    }
}