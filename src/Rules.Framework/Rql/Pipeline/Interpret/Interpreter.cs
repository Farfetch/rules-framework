namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Statements;
    using Rules.Framework.Rql.Tokens;

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

        public async Task<object> VisitComposedConditionExpression(ComposedConditionExpression expression)
        {
            var logicalOperatorName = (string)await expression.LogicalOperator.Accept(this).ConfigureAwait(false);
            var logicalOperator = Enum.Parse<LogicalOperators>(logicalOperatorName, ignoreCase: true);

            var childConditions = expression.ChildConditions;
            var length = childConditions.Length;
            var childConditionNodes = new IConditionNode<TConditionType>[length];
            for (int i = 0; i < length; i++)
            {
                childConditionNodes[i] = (IConditionNode<TConditionType>)await childConditions[i].Accept(this).ConfigureAwait(false);
            }

            return new ComposedConditionNode<TConditionType>(logicalOperator, childConditionNodes);
        }

        public async Task<object> VisitConditionGroupingExpression(ConditionGroupingExpression expression)
            => await expression.RootCondition.Accept(this).ConfigureAwait(false);

        public async Task<IResult> VisitCreateStatement(CreateStatement createStatement)
        {
            var rql = this.reverseRqlBuilder.BuildRql(createStatement);
            var ruleResult = await this.CreateRuleAsync(createStatement).ConfigureAwait(false);
            if (!ruleResult.IsSuccess)
            {
                ThrowRuntimeException(rql, ruleResult.Errors, createStatement.BeginPosition, createStatement.EndPosition);
            }

            var ruleAddResult = await this.AddRuleAsync(createStatement, ruleResult.Rule).ConfigureAwait(false);
            if (!ruleAddResult.IsSuccess)
            {
                ThrowRuntimeException(rql, ruleAddResult.Errors, createStatement.BeginPosition, createStatement.EndPosition);
            }

            var resultSet = new ResultSet<TContentType, TConditionType>(rql, affectedRules: 1, new List<ResultSetLine<TContentType, TConditionType>>(0));
            return new ResultSetStatementResult<TContentType, TConditionType>(resultSet);
        }

        public async Task<IResult> VisitDefinitionStatement(DefinitionStatement definitionStatement)
            => await definitionStatement.Definition.Accept(this).ConfigureAwait(false);

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

        public async Task<object> VisitInputConditionsExpression(InputConditionsExpression inputConditionsExpression)
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

        public Task<object> VisitKeywordExpression(KeywordExpression keywordExpression) => Task.FromResult<object>(keywordExpression.Keyword.Lexeme);

        public Task<object> VisitLiteralExpression(LiteralExpression literalExpression) => Task.FromResult(literalExpression.Value);

        public async Task<object> VisitMatchExpression(MatchExpression matchExpression)
        {
            var cardinality = (string)await matchExpression.Cardinality.Accept(this).ConfigureAwait(false);
            var contentTypeName = (string)await matchExpression.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName, ignoreCase: true);
            var matchDate = (DateTime)await matchExpression.MatchDate.Accept(this).ConfigureAwait(false);
            var conditions = (IEnumerable<Condition<TConditionType>>)await matchExpression.InputConditions.Accept(this).ConfigureAwait(false);

            if (string.Equals(cardinality, "ONE", StringComparison.OrdinalIgnoreCase))
            {
                var rule = await this.rulesEngine.MatchOneAsync(contentType, matchDate, conditions).ConfigureAwait(false);
                if (rule != null)
                {
                    return new[] { rule };
                }

                return Array.Empty<Rule<TContentType, TConditionType>>();
            }

            return await this.rulesEngine.MatchManyAsync(contentType, matchDate, conditions).ConfigureAwait(false);
        }

        public Task<object> VisitNoneExpression(NoneExpression noneExpression) => Task.FromResult<object>(result: null);

        public Task<IResult> VisitNoneStatement(NoneStatement statement) => Task.FromResult<IResult>(result: null);

        public Task<object> VisitOperatorExpression(OperatorExpression operatorExpression)
            => Task.FromResult<object>(result: operatorExpression.Token.Type switch
            {
                TokenType.EQUAL => Operators.Equal,
                TokenType.NOT_EQUAL => Operators.NotEqual,
                TokenType.GREATER_THAN => Operators.GreaterThan,
                TokenType.GREATER_THAN_OR_EQUAL => Operators.GreaterThanOrEqual,
                TokenType.LESS_THAN => Operators.LesserThan,
                TokenType.LESS_THAN_OR_EQUAL => Operators.LesserThanOrEqual,
                TokenType.IN => Operators.In,
                TokenType.NOT_IN => Operators.NotIn,
                _ => throw new NotSupportedException($"The token type '{operatorExpression.Token.Type}' is not supported as valid operator."),
            });

        public Task<object> VisitPlaceholderExpression(PlaceholderExpression placeholderExpression) => Task.FromResult(placeholderExpression.Token.Literal);

        public async Task<object> VisitPriorityOptionExpression(PriorityOptionExpression priorityOptionExpression)
        {
            var option = (string)await priorityOptionExpression.PriorityOption.Accept(this).ConfigureAwait(false);
            return option switch
            {
                "TOP" => RuleAddPriorityOption.AtTop,
                "BOTTOM" => RuleAddPriorityOption.AtBottom,
                "NAME" => RuleAddPriorityOption.ByRuleName((string)await priorityOptionExpression.Argument.Accept(this).ConfigureAwait(false)),
                "NUMBER" => RuleAddPriorityOption.ByPriorityNumber((int)await priorityOptionExpression.Argument.Accept(this).ConfigureAwait(false)),
                _ => throw new NotSupportedException($"The option '{option}' is not supported"),
            };
        }

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
            var conditions = (IEnumerable<Condition<TConditionType>>)await searchExpression.InputConditions.Accept(this).ConfigureAwait(false);
            var searchArgs = new SearchArgs<TContentType, TConditionType>(contentType, dateBegin, dateEnd)
            {
                Conditions = conditions,
                ExcludeRulesWithoutSearchConditions = true,
            };

            return await this.rulesEngine.SearchAsync(searchArgs).ConfigureAwait(false);
        }

        public async Task<object> VisitValueConditionExpression(ValueConditionExpression valueConditionExpression)
        {
            var conditionTypeName = (string)await valueConditionExpression.Left.Accept(this).ConfigureAwait(false);
            var conditionType = (TConditionType)Enum.Parse(typeof(TConditionType), conditionTypeName, ignoreCase: true);
            var @operator = (Operators)await valueConditionExpression.Operator.Accept(this).ConfigureAwait(false);
            var rightOperand = await valueConditionExpression.Right.Accept(this).ConfigureAwait(false);
            var dataType = rightOperand switch
            {
                int _ or IEnumerable<int> _ => DataTypes.Integer,
                bool _ or IEnumerable<bool> _ => DataTypes.Boolean,
                decimal _ or IEnumerable<decimal> _ => DataTypes.Decimal,
                string _ or IEnumerable<string> _ => DataTypes.String,
                _ => throw new NotSupportedException(),
            };
            return new ValueConditionNode<TConditionType>(dataType, conditionType, @operator, rightOperand);
        }

        private static void ThrowRuntimeException(string rql, IEnumerable<string> errors, RqlSourcePosition beginPosition, RqlSourcePosition endPosition)
        {
            string separator = $"{Environment.NewLine}\t - ";
            var errorsText = string.Join(separator, errors);
            throw new RuntimeException(
                $"Errors have occurred while creating new rule:{separator}{errorsText}",
                rql,
                beginPosition,
                endPosition);
        }

        private async Task<RuleOperationResult> AddRuleAsync(CreateStatement createStatement, Rule<TContentType, TConditionType> rule)
        {
            RuleAddPriorityOption ruleAddPriorityOption = null;
            if (createStatement.PriorityOption != null)
            {
                ruleAddPriorityOption = (RuleAddPriorityOption)await createStatement.PriorityOption.Accept(this).ConfigureAwait(false);
            }
            else
            {
                switch (this.rulesEngine.GetPriorityCriteria())
                {
                    case PriorityCriterias.TopmostRuleWins:
                        ruleAddPriorityOption = RuleAddPriorityOption.AtTop;
                        break;

                    case PriorityCriterias.BottommostRuleWins:
                        ruleAddPriorityOption = RuleAddPriorityOption.AtBottom;
                        break;
                }
            }

            return await this.rulesEngine.AddRuleAsync(rule, ruleAddPriorityOption).ConfigureAwait(false);
        }

        private async Task<RuleBuilderResult<TContentType, TConditionType>> CreateRuleAsync(CreateStatement createStatement)
        {
            var ruleName = (string)await createStatement.RuleName.Accept(this).ConfigureAwait(false);
            var contentTypeName = (string)await createStatement.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName, ignoreCase: true);
            var content = (string)await createStatement.Content.Accept(this).ConfigureAwait(false);
            var dateBegin = (DateTime)await createStatement.DateBegin.Accept(this).ConfigureAwait(false);
            DateTime? dateEnd = null;
            if (createStatement.DateEnd != null)
            {
                dateEnd = (DateTime)await createStatement.DateEnd.Accept(this).ConfigureAwait(false);
            }

            IConditionNode<TConditionType> condition = null;
            if (createStatement.Condition != null)
            {
                condition = (IConditionNode<TConditionType>)await createStatement.Condition.Accept(this).ConfigureAwait(false);
            }

            var ruleBuilder = RuleBuilder.NewRule<TContentType, TConditionType>()
                .WithName(ruleName)
                .WithDatesInterval(dateBegin, dateEnd)
                .WithContent(contentType, content);

            if (condition != null)
            {
                ruleBuilder.WithCondition(condition);
            }

            return ruleBuilder.Build();
        }
    }
}