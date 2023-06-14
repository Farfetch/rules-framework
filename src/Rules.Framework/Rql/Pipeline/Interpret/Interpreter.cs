namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Builder;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Rql.Expressions;
    using Rules.Framework.Rql.Statements;
    using Rules.Framework.Rql.Tokens;
    using Rules.Framework.Source;

    internal class Interpreter<TContentType, TConditionType> : IInterpreter, IExpressionVisitor<Task<object>>, IStatementVisitor<Task<IResult>>
    {
        private readonly IReverseRqlBuilder reverseRqlBuilder;
        private readonly IRulesEngine<TContentType, TConditionType> rulesEngine;
        private readonly IRulesSource<TContentType, TConditionType> rulesSource;
        private bool disposedValue;
        private IRuntimeEnvironment runtimeEnvironment;

        public Interpreter(
            IRulesEngine<TContentType, TConditionType> rulesEngine,
            IRulesSource<TContentType, TConditionType> rulesSource,
            IRuntimeEnvironment environment,
            IReverseRqlBuilder reverseRqlBuilder)
        {
            this.rulesEngine = rulesEngine;
            this.rulesSource = rulesSource;
            this.runtimeEnvironment = environment;
            this.reverseRqlBuilder = reverseRqlBuilder;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<object> InterpretAsync(IReadOnlyList<Statement> statements)
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

        public async Task<IResult> VisitActivationStatement(ActivationStatement activationStatement)
        {
            var rql = this.reverseRqlBuilder.BuildRql(activationStatement);
            var ruleName = (string)await activationStatement.RuleName.Accept(this).ConfigureAwait(false);
            var contentTypeName = (string)await activationStatement.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName, ignoreCase: true);

            var rules = await this.rulesSource.GetRulesFilteredAsync(new GetRulesFilteredArgs<TContentType>
            {
                ContentType = contentType,
                Name = ruleName,
            }).ConfigureAwait(false);

            if (!rules.Any())
            {
                throw CreateRuntimeException(
                    rql,
                    new[] { $"No such rule with name '{ruleName}' and content type '{contentTypeName}' was found." },
                    activationStatement.BeginPosition,
                    activationStatement.EndPosition);
            }

            var rule = rules.First();
            if (rule.Active)
            {
                List<RulesSetResultLine<TContentType, TConditionType>> emptyLines = new List<RulesSetResultLine<TContentType, TConditionType>>(0);
                var emptyResultSet = new RulesSetResult<TContentType, TConditionType>(rql, 0, emptyLines);
                return new RulesSetStatementResult<TContentType, TConditionType>(emptyResultSet);
            }

            var ruleActivationResult = await this.rulesEngine.ActivateRuleAsync(rule).ConfigureAwait(false);
            if (!ruleActivationResult.IsSuccess)
            {
                throw CreateRuntimeException(
                    rql,
                    ruleActivationResult.Errors,
                    activationStatement.BeginPosition,
                    activationStatement.EndPosition);
            }

            var lines = new List<RulesSetResultLine<TContentType, TConditionType>>(1) { new RulesSetResultLine<TContentType, TConditionType>(1, rule) };
            var resultSet = new RulesSetResult<TContentType, TConditionType>(rql, 1, lines);
            return new RulesSetStatementResult<TContentType, TConditionType>(resultSet);
        }

        public async Task<object> VisitCallExpression(CallExpression callExpression)
        {
            var rql = this.reverseRqlBuilder.BuildRql(callExpression);
            string callableName = callExpression.Identifier.Lexeme.ToUpperInvariant();
            var callee = this.runtimeEnvironment.Get(callableName);
            if (callee is not IRqlCallable)
            {
                throw CreateRuntimeException(
                    rql,
                    new[] { $"'{callableName}' is not a callable identifier." },
                    callExpression.BeginPosition,
                    callExpression.EndPosition);
            }

            var callable = (IRqlCallable)callee;
            int argumentsLength = callExpression.Arguments.Length;
            if (argumentsLength != callable.Arity)
            {
                throw CreateRuntimeException(
                    rql,
                    new[] { FormattableString.Invariant($"'{callableName}' expects {callable.Arity} argument(s) but {argumentsLength} were provided.") },
                    callExpression.BeginPosition, callExpression.EndPosition);
            }

            object[] arguments = new object[argumentsLength];
            for (int i = 0; i < argumentsLength; i++)
            {
                arguments[i] = await callExpression.Arguments[i].Accept(this).ConfigureAwait(false);
            }

            return callable.Call(this, arguments);
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
                throw CreateRuntimeException(rql,
                    ruleResult.Errors,
                    createStatement.BeginPosition,
                    createStatement.EndPosition);
            }

            var ruleAddResult = await this.AddRuleAsync(createStatement, ruleResult.Rule).ConfigureAwait(false);
            if (!ruleAddResult.IsSuccess)
            {
                throw CreateRuntimeException(
                    rql,
                    ruleAddResult.Errors,
                    createStatement.BeginPosition,
                    createStatement.EndPosition);
            }

            var lines = new List<RulesSetResultLine<TContentType, TConditionType>>(1)
            {
                new RulesSetResultLine<TContentType, TConditionType>(1, ruleResult.Rule),
            };
            var resultSet = new RulesSetResult<TContentType, TConditionType>(rql, affectedRules: 1, lines);
            return new RulesSetStatementResult<TContentType, TConditionType>(resultSet);
        }

        public async Task<IResult> VisitDeactivationStatement(DeactivationStatement deactivationStatement)
        {
            var rql = this.reverseRqlBuilder.BuildRql(deactivationStatement);
            var ruleName = (string)await deactivationStatement.RuleName.Accept(this).ConfigureAwait(false);
            var contentTypeName = (string)await deactivationStatement.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName, ignoreCase: true);

            var rules = await this.rulesSource.GetRulesFilteredAsync(new GetRulesFilteredArgs<TContentType>
            {
                ContentType = contentType,
                Name = ruleName,
            }).ConfigureAwait(false);

            if (!rules.Any())
            {
                throw CreateRuntimeException(
                    rql,
                    new[] { $"No such rule with name '{ruleName}' and content type '{contentTypeName}' was found." },
                    deactivationStatement.BeginPosition,
                    deactivationStatement.EndPosition);
            }

            var rule = rules.First();
            if (!rule.Active)
            {
                List<RulesSetResultLine<TContentType, TConditionType>> emptyLines = new List<RulesSetResultLine<TContentType, TConditionType>>(0);
                var emptyResultSet = new RulesSetResult<TContentType, TConditionType>(rql, 0, emptyLines);
                return new RulesSetStatementResult<TContentType, TConditionType>(emptyResultSet);
            }

            var ruleDeactivationResult = await this.rulesEngine.DeactivateRuleAsync(rule).ConfigureAwait(false);
            if (!ruleDeactivationResult.IsSuccess)
            {
                throw CreateRuntimeException(
                    rql,
                    ruleDeactivationResult.Errors,
                    deactivationStatement.BeginPosition,
                    deactivationStatement.EndPosition);
            }

            var lines = new List<RulesSetResultLine<TContentType, TConditionType>>(1) { new RulesSetResultLine<TContentType, TConditionType>(1, rule) };
            var resultSet = new RulesSetResult<TContentType, TConditionType>(rql, 1, lines);
            return new RulesSetStatementResult<TContentType, TConditionType>(resultSet);
        }

        public async Task<IResult> VisitDefinitionStatement(RuleDefinitionStatement definitionStatement)
            => await definitionStatement.Definition.Accept(this).ConfigureAwait(false);

        public async Task<object> VisitInputConditionExpression(InputConditionExpression inputConditionExpression)
        {
            var conditionTypeName = (string)await inputConditionExpression.Left.Accept(this).ConfigureAwait(false);
            if (!Enum.TryParse(typeof(TConditionType), conditionTypeName, out var conditionType))
            {
                string rql = this.reverseRqlBuilder.BuildRql(inputConditionExpression);
                throw CreateRuntimeException(
                    rql,
                    new[] { FormattableString.Invariant($"Condition type of name '{conditionTypeName}' was not found.") },
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
            var inputConditions = await matchExpression.InputConditions.Accept(this).ConfigureAwait(false);
            var conditions = inputConditions is null ? Array.Empty<Condition<TConditionType>>() : (IEnumerable<Condition<TConditionType>>)inputConditions;

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

        public async Task<IResult> VisitProgrammableSubLanguageStatement(ProgrammableSubLanguageStatement programmableStatement)
        {
            var rql = this.reverseRqlBuilder.BuildRql(programmableStatement);
            var expressionResult = await programmableStatement.Expression.Accept(this).ConfigureAwait(false);
            return new ExpressionResult(rql, expressionResult);
        }

        public async Task<IResult> VisitQueryStatement(RuleQueryStatement matchStatement)
        {
            var rules = (IEnumerable<Rule<TContentType, TConditionType>>)await matchStatement.Query.Accept(this).ConfigureAwait(false);
            var resultSetLines = new List<RulesSetResultLine<TContentType, TConditionType>>();
            var line = 1;

            foreach (var rule in rules)
            {
                resultSetLines.Add(new RulesSetResultLine<TContentType, TConditionType>(line++, rule));
            }

            var statementRql = this.reverseRqlBuilder.BuildRql(matchStatement);
            var resultSet = new RulesSetResult<TContentType, TConditionType>(statementRql, 0, resultSetLines);
            return new RulesSetStatementResult<TContentType, TConditionType>(resultSet);
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

        public async Task<object> VisitUpdatableAttributeExpression(UpdatableAttributeExpression updatableAttributeExpression)
        {
            var updatableAttributeExpressionValue = await updatableAttributeExpression.UpdatableAttribute.Accept(this).ConfigureAwait(false);
            var rule = (Rule<TContentType, TConditionType>)this.runtimeEnvironment.Get(".rule");
            var rql = (string)this.runtimeEnvironment.Get(".rql");
            switch (updatableAttributeExpression.Kind)
            {
                case UpdatableAttributeKind.DateEnd:
                    rule.DateEnd = (DateTime?)updatableAttributeExpressionValue;
                    break;

                case UpdatableAttributeKind.PriorityOption:
                    var priorityOption = (RuleAddPriorityOption)updatableAttributeExpressionValue;
                    var contentType = rule.ContentContainer.ContentType;
                    var allContentTypeRulesLazy = new Lazy<Task<IEnumerable<Rule<TContentType, TConditionType>>>>(async () =>
                        await this.rulesSource.GetRulesAsync(new GetRulesArgs<TContentType>
                        {
                            ContentType = contentType,
                            DateBegin = DateTime.MinValue,
                            DateEnd = DateTime.MaxValue,
                        }).ConfigureAwait(false));
                    switch (priorityOption.PriorityOption)
                    {
                        case PriorityOptions.AtTop:
                            rule.Priority = 1;
                            break;

                        case PriorityOptions.AtBottom:
                            var allRules1 = await allContentTypeRulesLazy.Value.ConfigureAwait(false);
                            rule.Priority = allRules1.Max(r => r.Priority);
                            break;

                        case PriorityOptions.AtPriorityNumber:
                            rule.Priority = priorityOption.AtPriorityNumberOptionValue;
                            break;

                        case PriorityOptions.AtRuleName:
                            var allRules2 = await allContentTypeRulesLazy.Value.ConfigureAwait(false);
                            var targetPriorityRule = allRules2.FirstOrDefault(r => string.Equals(r.Name, priorityOption.AtRuleNameOptionValue, StringComparison.Ordinal));
                            if (targetPriorityRule is null)
                            {
                                throw CreateRuntimeException(
                                    rql,
                                    new[] { $"No such rule with name '{priorityOption.AtRuleNameOptionValue}' and content type '{contentType}' was found for target priority." },
                                    updatableAttributeExpression.BeginPosition,
                                    updatableAttributeExpression.EndPosition);
                            }

                            rule.Priority = targetPriorityRule.Priority;
                            break;

                        default:
                            throw new NotSupportedException($"The priority option '{priorityOption.PriorityOption}' is not supported.");
                    }
                    break;

                default:
                    throw new NotSupportedException($"The updatable attribute kind '{updatableAttributeExpression.Kind}' is not supported.");
            }

            return null;
        }

        public async Task<IResult> VisitUpdateStatement(UpdateStatement updateStatement)
        {
            var rql = this.reverseRqlBuilder.BuildRql(updateStatement);
            var ruleName = (string)await updateStatement.RuleName.Accept(this).ConfigureAwait(false);
            var contentTypeName = (string)await updateStatement.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName, ignoreCase: true);
            var rules = await this.rulesSource
                .GetRulesFilteredAsync(new GetRulesFilteredArgs<TContentType>
                {
                    Name = ruleName,
                    ContentType = contentType,
                }).ConfigureAwait(false);

            if (!rules.Any())
            {
                throw CreateRuntimeException(
                    rql,
                    new[] { $"No such rule with name '{ruleName}' and content type '{contentTypeName}' was found." },
                    updateStatement.BeginPosition,
                    updateStatement.EndPosition);
            }

            var rule = rules.First();
            using (this.ScopeRuntimeEnvironment())
            {
                this.runtimeEnvironment.Define(".rule", rule);
                this.runtimeEnvironment.Define(".rql", rql);

                var updatableAttributes = updateStatement.UpdatableAttributes;
                var updatableAttributesLength = updatableAttributes.Length;
                for (var i = 0; i < updatableAttributesLength; i++)
                {
                    _ = await updatableAttributes[i].Accept(this).ConfigureAwait(false);
                }
            }

            var ruleUpdateResult = await this.rulesEngine.UpdateRuleAsync(rule).ConfigureAwait(false);
            if (!ruleUpdateResult.IsSuccess)
            {
                throw CreateRuntimeException(rql, ruleUpdateResult.Errors, updateStatement.BeginPosition, updateStatement.EndPosition);
            }

            var lines = new List<RulesSetResultLine<TContentType, TConditionType>>(1)
            {
                new RulesSetResultLine<TContentType, TConditionType>(1, rule),
            };
            var resultSet = new RulesSetResult<TContentType, TConditionType>(rql, 1, lines);
            return new RulesSetStatementResult<TContentType, TConditionType>(resultSet);
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

        public async Task<IResult> VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
        {
            var rql = this.reverseRqlBuilder.BuildRql(variableDeclarationStatement);
            object assignable = null;
            if (variableDeclarationStatement.Assignable is not null)
            {
                assignable = await variableDeclarationStatement.Assignable.Accept(this).ConfigureAwait(false);
            }

            this.runtimeEnvironment.Define(variableDeclarationStatement.Name.Lexeme.ToUpperInvariant(), assignable);
            return new VoidStatementResult(rql);
        }

        public Task<object> VisitVariableExpression(VariableExpression variableExpression)
        {
            try
            {
                return Task.FromResult(this.runtimeEnvironment.Get(variableExpression.Token.Lexeme.ToUpperInvariant()));
            }
            catch (IllegalRuntimeEnvironmentAccessException ex)
            {
                var rql = this.reverseRqlBuilder.BuildRql(variableExpression);
                throw CreateRuntimeException(
                    rql,
                    new[] { ex.Message },
                    variableExpression.BeginPosition,
                    variableExpression.EndPosition);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.runtimeEnvironment.Dispose();
                    this.runtimeEnvironment = null;
                }

                disposedValue = true;
            }
        }

        private static Exception CreateRuntimeException(string rql, IEnumerable<string> errors, RqlSourcePosition beginPosition, RqlSourcePosition endPosition)
        {
            string separator = $"{Environment.NewLine}\t - ";
            var errorsText = string.Join(separator, errors);
            return new RuntimeException(
                $"Errors have occurred while executing sentence:{separator}{errorsText}",
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
            var content = await createStatement.Content.Accept(this).ConfigureAwait(false);
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

        private IDisposable ScopeRuntimeEnvironment()
        {
            var parentRuntimeEnvironment = this.runtimeEnvironment;
            var childRuntimeEnvironment = this.runtimeEnvironment.CreateScopedChildRuntimeEnvironment();
            this.runtimeEnvironment = parentRuntimeEnvironment;
            return childRuntimeEnvironment;
        }
    }
}