namespace Rules.Framework.Rql
{
    using System;
    using System.Linq;
    using System.Text;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Statements;

    internal class ReverseRqlBuilder : IReverseRqlBuilder, IExpressionVisitor<string>, IStatementVisitor<string>
    {
        private const char SPACE = ' ';

        public string BuildRql(Expression expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return expression.Accept(this);
        }

        public string BuildRql(Statement statement)
        {
            if (statement is null)
            {
                throw new ArgumentNullException(nameof(statement));
            }

            return statement.Accept(this);
        }

        public string VisitCardinalityExpression(CardinalityExpression expression)
            => $"{expression.CardinalityKeyword.Accept(this)} {expression.RuleKeyword.Accept(this)}";

        public string VisitInputConditionExpression(InputConditionExpression inputConditionExpression)
        {
            var left = inputConditionExpression.Left.Accept(this);
            var @operator = inputConditionExpression.Operator.Lexeme;
            var right = inputConditionExpression.Right.Accept(this);
            return $"{left} {@operator} {right}";
        }

        public string VisitKeywordExpression(KeywordExpression keywordExpression) => keywordExpression.Keyword.Lexeme;

        public string VisitLiteralExpression(LiteralExpression literalExpression) => literalExpression.Type switch
        {
            LiteralType.String => $"\"{literalExpression.Value}\"",
            LiteralType.Decimal or LiteralType.Integer or LiteralType.Bool => literalExpression.Value.ToString(),
            LiteralType.DateTime => $"\"{literalExpression.Value:yyyy-MM-ddTHH:mm:ssZ}\"",
            _ => throw new NotSupportedException($"The literal type '{literalExpression.Type}' is not supported."),
        };

        public string VisitMatchExpression(MatchExpression matchExpression)
        {
            var cardinality = matchExpression.Cardinality.Accept(this);
            var contentType = matchExpression.ContentType.Accept(this);
            var matchDate = matchExpression.MatchDate.Accept(this);

            var matchRqlBuilder = new StringBuilder("MATCH")
                .Append(SPACE)
                .Append(cardinality)
                .Append(SPACE)
                .Append("FOR")
                .Append(SPACE)
                .Append(contentType)
                .Append(SPACE)
                .Append("ON")
                .Append(SPACE)
                .Append(matchDate);

            if (matchExpression.InputConditions.Any())
            {
                matchRqlBuilder.Append(SPACE)
                    .Append("WITH {");

                var notFirst = false;
                foreach (var inputConditionExpression in matchExpression.InputConditions)
                {
                    if (notFirst)
                    {
                        matchRqlBuilder.Append(',');
                    }
                    else
                    {
                        notFirst = true;
                    }

                    var inputCondition = inputConditionExpression.Accept(this);
                    matchRqlBuilder.Append(SPACE)
                        .Append(inputCondition);
                }

                matchRqlBuilder.Append(SPACE)
                    .Append('}');
            }

            return matchRqlBuilder.ToString();
        }

        public string VisitQueryStatement(QueryStatement matchStatement) => $"{matchStatement.Query.Accept(this)};";

        public string VisitNoneExpression(NoneExpression noneExpression) => string.Empty;

        public string VisitNoneStatement(NoneStatement noneStatement) => string.Empty;

        public string VisitSearchExpression(SearchExpression searchExpression)
        {
            var contentType = searchExpression.ContentType.Accept(this);
            var dateBegin = searchExpression.DateBegin.Accept(this);
            var dateEnd = searchExpression.DateEnd.Accept(this);

            var searchRqlBuilder = new StringBuilder("SEARCH RULES")
                .Append(SPACE)
                .Append("FOR")
                .Append(SPACE)
                .Append(contentType)
                .Append(SPACE)
                .Append("ON RANGE")
                .Append(SPACE)
                .Append(dateBegin)
                .Append(SPACE)
                .Append("TO")
                .Append(SPACE)
                .Append(dateEnd);

            if (searchExpression.InputConditions.Any())
            {
                searchRqlBuilder.Append(SPACE)
                    .Append("WITH {");

                var notFirst = false;
                foreach (var inputConditionExpression in searchExpression.InputConditions)
                {
                    if (notFirst)
                    {
                        searchRqlBuilder.Append(',');
                    }
                    else
                    {
                        notFirst = true;
                    }

                    var inputCondition = inputConditionExpression.Accept(this);
                    searchRqlBuilder.Append(SPACE)
                        .Append(inputCondition);
                }

                searchRqlBuilder.Append(SPACE)
                    .Append('}');
            }

            return searchRqlBuilder.ToString();
        }
    }
}