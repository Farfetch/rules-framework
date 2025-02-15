namespace Rules.Framework.Rql
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Rules.Framework.Rql.Ast;
    using Rules.Framework.Rql.Ast.Expressions;
    using Rules.Framework.Rql.Ast.Segments;
    using Rules.Framework.Rql.Ast.Statements;

    internal class ReverseRqlBuilder : IReverseRqlBuilder, IExpressionVisitor<string>, ISegmentVisitor<string>, IStatementVisitor<string>
    {
        private const char SPACE = ' ';

        public string BuildRql(IAstElement astElement)
        {
            if (astElement is null)
            {
                throw new ArgumentNullException(nameof(astElement));
            }

            return astElement switch
            {
                Expression expression => expression.Accept(this),
                Segment segment => segment.Accept(this),
                Statement statement => statement.Accept(this),
                _ => throw new NotSupportedException($"The given AST element is not supported: {astElement.GetType().FullName}."),
            };
        }

        public string VisitAssignmentExpression(AssignmentExpression assignmentExpression)
        {
            var left = assignmentExpression.Left.Accept(this);
            var right = assignmentExpression.Right.Accept(this);
            return FormattableString.Invariant($"{left} {assignmentExpression.Assign.Lexeme} {right}");
        }

        public string VisitBinaryExpression(BinaryExpression binaryExpression)
            => FormattableString.Invariant($"{binaryExpression.LeftExpression.Accept(this)} {binaryExpression.OperatorSegment.Accept(this)} {binaryExpression.RightExpression.Accept(this)}");

        public string VisitCardinalitySegment(CardinalitySegment expression)
            => $"{expression.CardinalityKeyword.Accept(this)} {expression.RuleKeyword.Accept(this)}";

        public string VisitDatesIntervalSegment(DatesIntervalSegment datesIntervalSegment)
        {
            return new StringBuilder()
                .Append(datesIntervalSegment.SinceKeyword.Accept(this))
                .Append(SPACE)
                .Append(datesIntervalSegment.SinceDate.Accept(this))
                .Append(SPACE)
                .Append(datesIntervalSegment.UntilKeyword.Accept(this))
                .Append(SPACE)
                .Append(datesIntervalSegment.UntilDate.Accept(this))
                .ToString();
        }

        public string VisitExpressionStatement(ExpressionStatement expressionStatement)
            => $"{expressionStatement.Expression.Accept(this)};";

        public string VisitIdentifierExpression(IdentifierExpression identifierExpression) => identifierExpression.Identifier.Lexeme;

        public string VisitInputConditionSegment(InputConditionSegment inputConditionExpression)
        {
            var left = inputConditionExpression.Left.Accept(this);
            var @operator = inputConditionExpression.Operator.Lexeme;
            var right = inputConditionExpression.Right.Accept(this);
            return $"{left} {@operator} {right}";
        }

        public string VisitInputConditionsSegment(InputConditionsSegment inputConditionsExpression)
        {
            var inputConditionsRqlBuilder = new StringBuilder();
            if (inputConditionsExpression.InputConditions.Any())
            {
                inputConditionsRqlBuilder.Append(inputConditionsExpression.WhenKeyword.Accept(this))
                    .Append(SPACE)
                    .Append(inputConditionsExpression.BeginToken.Lexeme);

                var notFirst = false;
                foreach (var inputConditionExpression in inputConditionsExpression.InputConditions)
                {
                    if (notFirst)
                    {
                        inputConditionsRqlBuilder.Append(',');
                    }
                    else
                    {
                        notFirst = true;
                    }

                    var inputCondition = inputConditionExpression.Accept(this);
                    inputConditionsRqlBuilder.Append(SPACE)
                        .Append(inputCondition);
                }

                inputConditionsRqlBuilder.Append(SPACE)
                    .Append(inputConditionsExpression.EndToken.Lexeme);
            }

            return inputConditionsRqlBuilder.ToString();
        }

        public string VisitKeywordExpression(KeywordExpression keywordExpression) => keywordExpression.Keyword.Lexeme.ToUpperInvariant();

        public string VisitLiteralExpression(LiteralExpression literalExpression) => literalExpression.Type switch
        {
            LiteralType.String or LiteralType.Undefined => literalExpression.Token.Lexeme,
            LiteralType.Bool => literalExpression.Value.ToString().ToUpperInvariant(),
            LiteralType.Decimal or LiteralType.Integer => Convert.ToString(literalExpression.Value, CultureInfo.InvariantCulture),
            LiteralType.DateTime => $"${literalExpression.Value:yyyy-MM-ddTHH:mm:ssZ}$",
            _ => throw new NotSupportedException($"The literal type '{literalExpression.Type}' is not supported."),
        };

        public string VisitMatchDateSegment(MatchDateSegment matchDateSegment) => $"ON {matchDateSegment.MatchDate.Accept(this)}";

        public string VisitMatchExpression(MatchExpression matchExpression)
        {
            var match = matchExpression.MatchKeyword.Accept(this);
            var cardinality = matchExpression.Cardinality.Accept(this);
            var ruleset = matchExpression.Ruleset.Accept(this);
            var matchDate = matchExpression.MatchDate.Accept(this);
            var inputConditions = matchExpression.InputConditions.Accept(this);

            var matchRqlBuilder = new StringBuilder(match)
                .Append(SPACE)
                .Append(cardinality)
                .Append(SPACE)
                .Append(ruleset)
                .Append(SPACE)
                .Append(matchDate);

            if (!string.IsNullOrWhiteSpace(inputConditions))
            {
                matchRqlBuilder.Append(SPACE)
                    .Append(inputConditions);
            }

            return matchRqlBuilder.ToString();
        }

        public string VisitNewArrayExpression(NewArrayExpression newArrayExpression)
        {
            var stringBuilder = new StringBuilder(newArrayExpression.Array.Lexeme)
                .Append(SPACE)
                .Append(newArrayExpression.InitializerBeginToken.Lexeme);

            if (newArrayExpression.Size != Expression.None)
            {
                stringBuilder.Append(newArrayExpression.Size.Accept(this));
            }
            else
            {
                for (var i = 0; i < newArrayExpression.Values.Length; i++)
                {
                    stringBuilder.Append(SPACE)
                        .Append(newArrayExpression.Values[i].Accept(this));

                    if (i < newArrayExpression.Values.Length - 1)
                    {
                        stringBuilder.Append(',');
                    }
                }

                stringBuilder.Append(SPACE);
            }

            return stringBuilder.Append(newArrayExpression.InitializerEndToken.Lexeme)
                .ToString();
        }

        public string VisitNewObjectExpression(NewObjectExpression newObjectExpression)
        {
            var stringBuilder = new StringBuilder(newObjectExpression.Object.Lexeme);

            if (newObjectExpression.PropertyAssignments.Length > 0)
            {
                stringBuilder.AppendLine()
                    .Append('{');
                for (var i = 0; i < newObjectExpression.PropertyAssignments.Length; i++)
                {
                    var propertyAssignment = newObjectExpression.PropertyAssignments[i].Accept(this);
                    stringBuilder.AppendLine()
                        .Append(new string(' ', 4))
                    .Append(propertyAssignment);

                    if (i < newObjectExpression.PropertyAssignments.Length - 1)
                    {
                        stringBuilder.Append(',');
                    }
                }
                stringBuilder.AppendLine()
                .Append('}');
            }

            return stringBuilder.ToString();
        }

        public string VisitNoneExpression(NoneExpression noneExpression) => string.Empty;

        public string VisitNoneSegment(NoneSegment noneSegment) => string.Empty;

        public string VisitNoneStatement(NoneStatement noneStatement) => string.Empty;

        public string VisitOperatorSegment(OperatorSegment operatorExpression)
        {
#if NETSTANDARD2_1_OR_GREATER
            var separator = SPACE;
#else
            var separator = new string(SPACE, 1);
#endif
            return operatorExpression.Tokens.Select(t => t.Lexeme).Aggregate((t1, t2) => string.Join(separator, t1, t2));
        }

        public string VisitPlaceholderExpression(PlaceholderExpression placeholderExpression) => placeholderExpression.Token.Lexeme;

        public string VisitRulesetSegment(RulesetSegment rulesetSegment)
            => $"{rulesetSegment.ForKeyword.Accept(this)} {rulesetSegment.RulesetName.Accept(this)}";

        public string VisitSearchExpression(SearchExpression searchExpression)
        {
            var inputConditions = searchExpression.InputConditions.Accept(this);

            var searchRqlBuilder = new StringBuilder()
                .Append(searchExpression.SearchKeyword.Accept(this))
                .Append(SPACE)
                .Append(searchExpression.RulesKeyword.Accept(this))
                .Append(SPACE)
                .Append(searchExpression.Ruleset.Accept(this))
                .Append(SPACE)
                .Append(searchExpression.DatesInterval.Accept(this));

            if (!string.IsNullOrWhiteSpace(inputConditions))
            {
                searchRqlBuilder.Append(SPACE)
                    .Append(inputConditions);
            }

            return searchRqlBuilder.ToString();
        }

        public string VisitUnaryExpression(UnaryExpression unaryExpression) => $"{unaryExpression.Operator.Lexeme}{unaryExpression.Right.Accept(this)}";
    }
}