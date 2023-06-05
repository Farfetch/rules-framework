namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Statements;

    internal class Interpreter<TContentType, TConditionType> : IExpressionVisitor<Task<object>>, IStatementVisitor<Task<IResult>>
    {
        private readonly IReverseRqlBuilder reverseRqlBuilder;
        private readonly IRulesEngine<TContentType, TConditionType> rulesEngine;

        public Interpreter(IRulesEngine<TContentType, TConditionType> rulesEngine, IReverseRqlBuilder reverseRqlBuilder)
        {
            this.rulesEngine = rulesEngine;
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
                    if (statementResult != null)
                    {
                        interpretResult.AddStatementResult(statementResult);
                    }
                }
                catch (RuntimeException re)
                {
                    var errorStatementResult = new ErrorResult(re.Message, re.Rql, re.BeginPosition, re.EndPosition);
                    interpretResult.AddStatementResult(errorStatementResult);
                    break;
                }
            }

            return interpretResult;
        }

        public Task<object> VisitCardinalityExpression(CardinalityExpression expression) => expression.CardinalityKeyword.Accept(this);

        public async Task<object> VisitInputConditionExpression(InputConditionExpression inputConditionExpression)
        {
            var conditionTypeName = (string)await inputConditionExpression.Left.Accept(this).ConfigureAwait(false);
            if (!Enum.TryParse(typeof(TConditionType), conditionTypeName, out var conditionType))
            {
                throw new RuntimeException(
                    $"Condition type of name '{conditionTypeName}' was not found.",
                    this.reverseRqlBuilder.BuildRql(inputConditionExpression),
                    inputConditionExpression.BeginPosition,
                    inputConditionExpression.EndPosition);
            }

            var conditionValue = await inputConditionExpression.Right.Accept(this).ConfigureAwait(false);
            return new Condition<TConditionType>((TConditionType)conditionType, conditionValue);
        }

        public Task<object> VisitKeywordExpression(KeywordExpression keywordExpression) => Task.FromResult<object>(keywordExpression.Keyword.Lexeme);

        public Task<object> VisitLiteralExpression(LiteralExpression literalExpression) => Task.FromResult(literalExpression.Value);

        public async Task<object> VisitMatchExpression(MatchExpression matchExpression)
        {
            var cardinality = (string)await matchExpression.Cardinality.Accept(this).ConfigureAwait(false);
            var contentTypeName = (string)await matchExpression.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName, ignoreCase: true);
            var matchDate = (DateTime)await matchExpression.MatchDate.Accept(this).ConfigureAwait(false);

            var conditions = new List<Condition<TConditionType>>();
            foreach (var inputConditionExpression in matchExpression.InputConditions)
            {
                var condition = (Condition<TConditionType>)await inputConditionExpression.Accept(this).ConfigureAwait(false);
                conditions.Add(condition);
            }

            if (string.Equals(cardinality, "ONE", StringComparison.OrdinalIgnoreCase))
            {
                var rule = await this.rulesEngine.MatchOneAsync(contentType, matchDate, conditions).ConfigureAwait(false);
                if (rule != null)
                {
                    return new[] { rule };
                }

                return Enumerable.Empty<Rule<TContentType, TConditionType>>();
            }

            return await this.rulesEngine.MatchManyAsync(contentType, matchDate, conditions).ConfigureAwait(false);
        }

        public Task<object> VisitNoneExpression(NoneExpression noneExpression) => Task.FromResult<object>(result: null);

        public Task<IResult> VisitNoneStatement(NoneStatement statement) => Task.FromResult<IResult>(result: null);

        public async Task<IResult> VisitQueryStatement(QueryStatement matchStatement)
        {
            var rules = (IEnumerable<Rule<TContentType, TConditionType>>)await matchStatement.Query.Accept(this).ConfigureAwait(false);
            var resultSetLines = new List<ResultSetLine<TContentType, TConditionType>>();
            var line = 1;

            foreach (var rule in rules)
            {
                resultSetLines.Add(new ResultSetLine<TContentType, TConditionType>(line++, rule));
            }

            var statementRql = this.reverseRqlBuilder.BuildRql(matchStatement);
            var resultSet = new ResultSet<TContentType, TConditionType>(statementRql, 0, resultSetLines);
            return new ResultSetStatementResult<TContentType, TConditionType>(resultSet);
        }

        public async Task<object> VisitSearchExpression(SearchExpression searchExpression)
        {
            var contentTypeName = (string)await searchExpression.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName, ignoreCase: true);
            var dateBegin = (DateTime)await searchExpression.DateBegin.Accept(this).ConfigureAwait(false);
            var dateEnd = (DateTime)await searchExpression.DateEnd.Accept(this).ConfigureAwait(false);

            var conditions = new List<Condition<TConditionType>>();
            foreach (var inputConditionExpression in searchExpression.InputConditions)
            {
                var condition = (Condition<TConditionType>)await inputConditionExpression.Accept(this).ConfigureAwait(false);
                conditions.Add(condition);
            }

            var searchArgs = new SearchArgs<TContentType, TConditionType>(contentType, dateBegin, dateEnd)
            {
                Conditions = conditions,
                ExcludeRulesWithoutSearchConditions = true,
            };

            return await this.rulesEngine.SearchAsync(searchArgs).ConfigureAwait(false);
        }
    }
}