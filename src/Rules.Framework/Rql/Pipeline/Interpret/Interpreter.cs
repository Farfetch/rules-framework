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
    using Rules.Framework.Rql.Runtime;
    using Rules.Framework.Rql.Runtime.Types;
    using Rules.Framework.Rql.Statements;
    using Rules.Framework.Rql.Tokens;
    using Rules.Framework.Source;

    internal class Interpreter<TContentType, TConditionType> : IInterpreter, IExpressionVisitor<Task<object>>, IStatementVisitor<Task<IResult>>
    {
        private readonly IReverseRqlBuilder reverseRqlBuilder;
        private readonly IRulesEngine<TContentType, TConditionType> rulesEngine;
        private readonly IRulesSource<TContentType, TConditionType> rulesSource;
        private bool disposedValue;
        private IRuntime runtime;

        public Interpreter(
            IRulesEngine<TContentType, TConditionType> rulesEngine,
            IRulesSource<TContentType, TConditionType> rulesSource,
            IRuntime runtime,
            IReverseRqlBuilder reverseRqlBuilder)
        {
            this.rulesEngine = rulesEngine;
            this.rulesSource = rulesSource;
            this.runtime = runtime;
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
                    var errorStatementResult = new ErrorStatementResult(re.Message, re.Rql, re.BeginPosition, re.EndPosition);
                    interpretResult.AddStatementResult(errorStatementResult);
                    break;
                }
            }

            return interpretResult;
        }

        public async Task<object> VisitActivationExpression(ActivationExpression activationExpression)
        {
            var rql = this.reverseRqlBuilder.BuildRql(activationExpression);
            var ruleName = (RqlString)await activationExpression.RuleName.Accept(this).ConfigureAwait(false);
            var contentTypeName = (RqlString)await activationExpression.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName.Value, ignoreCase: true);

            var rules = await this.rulesSource.GetRulesFilteredAsync(new GetRulesFilteredArgs<TContentType>
            {
                ContentType = contentType,
                Name = ruleName.Value,
            }).ConfigureAwait(false);

            if (!rules.Any())
            {
                throw CreateRuntimeException(
                    rql,
                    new[] { $"No such rule with name '{ruleName.Value}' and content type '{contentTypeName.Value}' was found." },
                    activationExpression.BeginPosition,
                    activationExpression.EndPosition);
            }

            var rule = rules.First();
            if (rule.Active)
            {
                return new RqlArray(0);
            }

            var ruleActivationResult = await this.rulesEngine.ActivateRuleAsync(rule).ConfigureAwait(false);
            if (!ruleActivationResult.IsSuccess)
            {
                throw CreateRuntimeException(
                    rql,
                    ruleActivationResult.Errors,
                    activationExpression.BeginPosition,
                    activationExpression.EndPosition);
            }

            var result = new RqlArray(1);
            result.SetAtIndex(0, new RqlRule<TContentType, TConditionType>(rule));
            return result;
        }

        public async Task<object> VisitAssignExpression(AssignmentExpression assignmentExpression)
        {
            try
            {
                var variableName = (string)await assignmentExpression.Left.Accept(this).ConfigureAwait(false);
                var value = await assignmentExpression.Right.Accept(this).ConfigureAwait(false);
                this.runtime.Environment.Assign(variableName, value);
                return new RqlNothing();
            }
            catch (IllegalRuntimeEnvironmentAccessException ex)
            {
                var rql = this.reverseRqlBuilder.BuildRql(assignmentExpression);
                throw CreateRuntimeException(
                    rql,
                    new[] { ex.Message },
                    assignmentExpression.BeginPosition,
                    assignmentExpression.EndPosition);
            }
        }

        public async Task<object> VisitCallExpression(CallExpression callExpression)
        {
            var rql = this.reverseRqlBuilder.BuildRql(callExpression);
            try
            {
                var caller = (IRuntimeValue)await callExpression.Instance.Accept(this).ConfigureAwait(false);
                string callableName = (string)await callExpression.Name.Accept(this).ConfigureAwait(false);
                int argumentsLength = callExpression.Arguments.Length;
                var arguments = new IRuntimeValue[argumentsLength];
                for (int i = 0; i < argumentsLength; i++)
                {
                    arguments[i] = (IRuntimeValue)await callExpression.Arguments[i].Accept(this).ConfigureAwait(false);
                }

                var callable = caller is RqlNothing && callExpression.Instance == Expression.None
                    ? this.runtime.CallableTable.ResolveCallable(callableName, arguments.Select(a => a.Type).ToArray())
                    : this.runtime.CallableTable.ResolveCallable(caller.Type, callableName, arguments.Select(a => a.Type).ToArray());
                if (callable is null)
                {
                    throw CreateRuntimeException(
                        rql,
                        new[] { $"'{callableName}' is not a valid callable." },
                        callExpression.BeginPosition,
                        callExpression.EndPosition);
                }

                if (argumentsLength != callable.Arity)
                {
                    throw CreateRuntimeException(
                        rql,
                        new[] { FormattableString.Invariant($"'{callableName}' expects {callable.Arity} argument(s) but {argumentsLength} were provided.") },
                        callExpression.BeginPosition, callExpression.EndPosition);
                }

                return callable.Call(this, caller, arguments);
            }
            catch (CallableTableException ex)
            {
                throw CreateRuntimeException(
                    rql,
                    new[] { $"{ex.Message} - {ex.CallableSpace}.{ex.CallableName}({ex.CallableParameterTypes.Aggregate((p1, p2) => $"{p1}, {p2}")})" },
                    callExpression.BeginPosition,
                    callExpression.EndPosition);
            }
        }

        public Task<object> VisitCardinalityExpression(CardinalityExpression expression) => expression.CardinalityKeyword.Accept(this);

        public async Task<object> VisitComposedConditionExpression(ComposedConditionExpression expression)
        {
            var logicalOperatorName = (RqlString)await expression.LogicalOperator.Accept(this).ConfigureAwait(false);
            var logicalOperator = Enum.Parse<LogicalOperators>(logicalOperatorName.Value, ignoreCase: true);

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

        public async Task<object> VisitCreateExpression(CreateExpression createExpression)
        {
            var rql = this.reverseRqlBuilder.BuildRql(createExpression);
            var ruleResult = await this.CreateRuleAsync(createExpression).ConfigureAwait(false);
            if (!ruleResult.IsSuccess)
            {
                throw CreateRuntimeException(rql,
                    ruleResult.Errors,
                    createExpression.BeginPosition,
                    createExpression.EndPosition);
            }

            var rule = ruleResult.Rule;
            var ruleAddResult = await this.AddRuleAsync(createExpression, rule).ConfigureAwait(false);
            if (!ruleAddResult.IsSuccess)
            {
                throw CreateRuntimeException(
                    rql,
                    ruleAddResult.Errors,
                    createExpression.BeginPosition,
                    createExpression.EndPosition);
            }

            var result = new RqlArray(1);
            result.SetAtIndex(0, new RqlRule<TContentType, TConditionType>(rule));
            return result;
        }

        public async Task<object> VisitDeactivationExpression(DeactivationExpression deactivationExpression)
        {
            var rql = this.reverseRqlBuilder.BuildRql(deactivationExpression);
            var ruleName = (RqlString)await deactivationExpression.RuleName.Accept(this).ConfigureAwait(false);
            var contentTypeName = (RqlString)await deactivationExpression.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName.Value, ignoreCase: true);

            var rules = await this.rulesSource.GetRulesFilteredAsync(new GetRulesFilteredArgs<TContentType>
            {
                ContentType = contentType,
                Name = ruleName.Value,
            }).ConfigureAwait(false);

            if (!rules.Any())
            {
                throw CreateRuntimeException(
                    rql,
                    new[] { $"No such rule with name '{ruleName.Value}' and content type '{contentTypeName.Value}' was found." },
                    deactivationExpression.BeginPosition,
                    deactivationExpression.EndPosition);
            }

            var rule = rules.First();
            if (!rule.Active)
            {
                return new RqlArray(0);
            }

            var ruleDeactivationResult = await this.rulesEngine.DeactivateRuleAsync(rule).ConfigureAwait(false);
            if (!ruleDeactivationResult.IsSuccess)
            {
                throw CreateRuntimeException(
                    rql,
                    ruleDeactivationResult.Errors,
                    deactivationExpression.BeginPosition,
                    deactivationExpression.EndPosition);
            }

            var result = new RqlArray(1);
            result.SetAtIndex(0, new RqlRule<TContentType, TConditionType>(rule));
            return result;
        }

        public async Task<IResult> VisitExpressionStatement(ExpressionStatement programmableStatement)
        {
            var rql = this.reverseRqlBuilder.BuildRql(programmableStatement);
            var expressionResult = await programmableStatement.Expression.Accept(this).ConfigureAwait(false);
            return new ExpressionStatementResult(rql, expressionResult);
        }

        public Task<object> VisitIdentifierExpression(IdentifierExpression identifierExpression)
            => Task.FromResult<object>(identifierExpression.Identifier.UnescapedLexeme);

        public async Task<object> VisitIndexerGetExpression(IndexerGetExpression indexerGetExpression)
        {
            var rql = this.reverseRqlBuilder.BuildRql(indexerGetExpression);
            var instance = (IRuntimeValue)await indexerGetExpression.Instance.Accept(this).ConfigureAwait(false);
            if (instance.Type == RqlTypes.Any)
            {
                instance = ((RqlAny)instance).Unwrap();
            }

            if (instance is not IIndexerGet indexerGet)
            {
                var type = instance.Type == RqlTypes.Any ? ((RqlAny)instance).UnderlyingType.Name : instance.Type.Name;
                throw CreateRuntimeException(
                    rql,
                    new[] { $"Type '{type}' is not a valid indexer target." },
                    indexerGetExpression.BeginPosition,
                    indexerGetExpression.EndPosition);
            }

            var index = (RqlInteger)await indexerGetExpression.Index.Accept(this).ConfigureAwait(false);
            if (index < 0 || index >= indexerGet.Size)
            {
                var type = instance.Type == RqlTypes.Any ? ((RqlAny)instance).UnderlyingType.Name : instance.Type.Name;
                throw CreateRuntimeException(
                    rql,
                    new[] { $"Index '{index}' is out of bounds for instance of '{type}'." },
                    indexerGetExpression.BeginPosition,
                    indexerGetExpression.EndPosition);
            }

            return indexerGet.GetAtIndex(index);
        }

        public async Task<object> VisitIndexerSetExpression(IndexerSetExpression indexerSetExpression)
        {
            var rql = this.reverseRqlBuilder.BuildRql(indexerSetExpression);
            var instance = (IRuntimeValue)await indexerSetExpression.Instance.Accept(this).ConfigureAwait(false);
            if (instance.Type == RqlTypes.Any)
            {
                instance = ((RqlAny)instance).Unwrap();
            }

            if (instance is not IIndexerSet indexerSet)
            {
                var type = instance.Type == RqlTypes.Any ? ((RqlAny)instance).UnderlyingType.Name : instance.Type.Name;
                throw CreateRuntimeException(
                    rql,
                    new[] { $"Type '{type}' is not a valid indexer target." },
                    indexerSetExpression.BeginPosition,
                    indexerSetExpression.EndPosition);
            }

            var index = (RqlInteger)await indexerSetExpression.Index.Accept(this).ConfigureAwait(false);
            if (index < 0 || index >= indexerSet.Size)
            {
                var type = instance.Type == RqlTypes.Any ? ((RqlAny)instance).UnderlyingType.Name : instance.Type.Name;
                throw CreateRuntimeException(
                    rql,
                    new[] { $"Index '{index}' is out of bounds for instance of '{type}'." },
                    indexerSetExpression.BeginPosition,
                    indexerSetExpression.EndPosition);
            }

            var value = (IRuntimeValue)await indexerSetExpression.Value.Accept(this).ConfigureAwait(false);
            return indexerSet.SetAtIndex(index, new RqlAny(value));
        }

        public async Task<object> VisitInputConditionExpression(InputConditionExpression inputConditionExpression)
        {
            var conditionTypeName = (RqlString)await inputConditionExpression.Left.Accept(this).ConfigureAwait(false);
            if (!Enum.TryParse(typeof(TConditionType), conditionTypeName.Value, out var conditionType))
            {
                string rql = this.reverseRqlBuilder.BuildRql(inputConditionExpression);
                throw CreateRuntimeException(
                    rql,
                    new[] { FormattableString.Invariant($"Condition type of name '{conditionTypeName}' was not found.") },
                    inputConditionExpression.BeginPosition,
                    inputConditionExpression.EndPosition);
            }

            var conditionValue = (IRuntimeValue)await inputConditionExpression.Right.Accept(this).ConfigureAwait(false);
            return new Condition<TConditionType>((TConditionType)conditionType, conditionValue.RuntimeValue);
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

        public Task<object> VisitKeywordExpression(KeywordExpression keywordExpression)
            => Task.FromResult<object>(new RqlString(keywordExpression.Keyword.Lexeme));

        public Task<object> VisitLiteralExpression(LiteralExpression literalExpression)
        {
            return Task.FromResult<object>(literalExpression.Type switch
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
                LiteralType.DateTime => new RqlDate((DateTime)literalExpression.Value),
                LiteralType.Undefined => new RqlNothing(),
                _ when literalExpression.Value is null => new RqlNothing(),
                _ => throw new NotSupportedException($"Literal with type '{literalExpression.Type}' is not supported."),
            });
        }

        public async Task<object> VisitMatchExpression(MatchExpression matchExpression)
        {
            var cardinality = (RqlString)await matchExpression.Cardinality.Accept(this).ConfigureAwait(false);
            var contentTypeName = (RqlString)await matchExpression.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName.Value, ignoreCase: true);
            var matchDate = (RqlDate)await matchExpression.MatchDate.Accept(this).ConfigureAwait(false);
            var inputConditions = await matchExpression.InputConditions.Accept(this).ConfigureAwait(false);
            var conditions = inputConditions is null ? Array.Empty<Condition<TConditionType>>() : (IEnumerable<Condition<TConditionType>>)inputConditions;

            if (string.Equals(cardinality.Value, "ONE", StringComparison.OrdinalIgnoreCase))
            {
                var rule = await this.rulesEngine.MatchOneAsync(contentType, matchDate.Value, conditions).ConfigureAwait(false);
                if (rule != null)
                {
                    var rqlArrayOne = new RqlArray(1);
                    rqlArrayOne.SetAtIndex(0, new RqlRule<TContentType, TConditionType>(rule));
                    return rqlArrayOne;
                }

                return new RqlArray(0);
            }

            var rules = await this.rulesEngine.MatchManyAsync(contentType, matchDate.Value, conditions).ConfigureAwait(false);
            var rqlArrayAll = new RqlArray(rules.Count());
            var i = 0;
            foreach (var rule in rules)
            {
                rqlArrayAll.SetAtIndex(i++, new RqlRule<TContentType, TConditionType>(rule));
            }

            return rqlArrayAll;
        }

        public async Task<object> VisitNewArrayExpression(NewArrayExpression newArrayExpression)
        {
            var sizeValue = (IRuntimeValue)await newArrayExpression.Size.Accept(this).ConfigureAwait(false);
            var size = sizeValue is RqlInteger integer ? integer.Value : newArrayExpression.Values.Length;
            var rqlArray = new RqlArray(size);

            if (newArrayExpression.Values.Length > 0)
            {
                for (var i = 0; i < size; i++)
                {
                    var value = (IRuntimeValue)await newArrayExpression.Values[i].Accept(this).ConfigureAwait(false);
                    rqlArray.Value[i] = new RqlAny(value);
                }
            }
            else
            {
                for (var i = 0; i < size; i++)
                {
                    rqlArray.Value[i] = new RqlNothing();
                }
            }

            return rqlArray;
        }

        public async Task<object> VisitNewObjectExpression(NewObjectExpression newObjectExpression)
        {
            var rqlObject = new RqlObject();
            var propertyAssignments = newObjectExpression.PropertyAssignments;
            for (int i = 0; i < propertyAssignments.Length; i++)
            {
                var assignment = (AssignmentExpression)propertyAssignments[i];
                var left = (string)await assignment.Left.Accept(this).ConfigureAwait(false);
                var right = (IRuntimeValue)await assignment.Right.Accept(this).ConfigureAwait(false);
                rqlObject.SetPropertyValue(left, new RqlAny(right));
            }

            return rqlObject;
        }

        public Task<object> VisitNoneExpression(NoneExpression noneExpression) => Task.FromResult<object>(new RqlNothing());

        public Task<IResult> VisitNoneStatement(NoneStatement statement) => Task.FromResult<IResult>(new ExpressionStatementResult(string.Empty, new RqlNothing()));

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

        public Task<object> VisitPlaceholderExpression(PlaceholderExpression placeholderExpression)
            => Task.FromResult<object>(new RqlString((string)placeholderExpression.Token.Literal));

        public async Task<object> VisitPriorityOptionExpression(PriorityOptionExpression priorityOptionExpression)
        {
            var option = (RqlString)await priorityOptionExpression.PriorityOption.Accept(this).ConfigureAwait(false);
            switch (option.Value.ToUpperInvariant())
            {
                case "TOP":
                    return RuleAddPriorityOption.AtTop;

                case "BOTTOM":
                    return RuleAddPriorityOption.AtBottom;

                case "NAME":
                    RqlString ruleName = (RqlString)await priorityOptionExpression.Argument.Accept(this).ConfigureAwait(false);
                    return RuleAddPriorityOption.ByRuleName(ruleName.Value);

                case "NUMBER":
                    RqlInteger priorityNumber = (RqlInteger)await priorityOptionExpression.Argument.Accept(this).ConfigureAwait(false);
                    return RuleAddPriorityOption.ByPriorityNumber((int)priorityNumber.Value);

                default:
                    throw new NotSupportedException($"The option '{option}' is not supported");
            }
        }

        public async Task<object> VisitPropertyGetExpression(PropertyGetExpression propertyGetExpression)
        {
            var rql = this.reverseRqlBuilder.BuildRql(propertyGetExpression);
            var instance = (IRuntimeValue)await propertyGetExpression.Instance.Accept(this).ConfigureAwait(false);
            if (instance.Type == RqlTypes.Any)
            {
                instance = ((RqlAny)instance).Unwrap();
            }

            if (instance is IPropertyGet propertyGet)
            {
                var propertyName = (string)await propertyGetExpression.Name.Accept(this).ConfigureAwait(false);
                if (propertyGet.TryGetPropertyValue(propertyName, out var propertyValue))
                {
                    return propertyValue;
                }

                throw CreateRuntimeException(
                    rql,
                    new[] { $"Instance of '{nameof(RqlObject)}' does not contain property '{propertyName}'." },
                    propertyGetExpression.BeginPosition,
                    propertyGetExpression.EndPosition);
            }

            throw CreateRuntimeException(
                rql,
                new[] { $"Instance does not contain properties." },
                propertyGetExpression.BeginPosition,
                propertyGetExpression.EndPosition);
        }

        public async Task<object> VisitPropertySetExpression(PropertySetExpression propertySetExpression)
        {
            var rql = this.reverseRqlBuilder.BuildRql(propertySetExpression);
            var instance = (IRuntimeValue)await propertySetExpression.Instance.Accept(this).ConfigureAwait(false);
            if (instance.Type == RqlTypes.Any)
            {
                instance = ((RqlAny)instance).Unwrap();
            }

            if (instance is IPropertySet propertySet)
            {
                var propertyName = (string)await propertySetExpression.Name.Accept(this).ConfigureAwait(false);
                var value = (IRuntimeValue)await propertySetExpression.Value.Accept(this).ConfigureAwait(false);
                propertySet.SetPropertyValue(propertyName, new RqlAny(value));
                return new RqlNothing();
            }

            throw CreateRuntimeException(
                rql,
                new[] { $"Instance of does not contain properties." },
                propertySetExpression.BeginPosition,
                propertySetExpression.EndPosition);
        }

        public async Task<object> VisitSearchExpression(SearchExpression searchExpression)
        {
            var contentTypeName = (RqlString)await searchExpression.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName.Value, ignoreCase: true);
            var dateBegin = (RqlDate)await searchExpression.DateBegin.Accept(this).ConfigureAwait(false);
            var dateEnd = (RqlDate)await searchExpression.DateEnd.Accept(this).ConfigureAwait(false);
            var conditions = (IEnumerable<Condition<TConditionType>>)await searchExpression.InputConditions.Accept(this).ConfigureAwait(false);
            var searchArgs = new SearchArgs<TContentType, TConditionType>(contentType, dateBegin.Value, dateEnd.Value)
            {
                Conditions = conditions,
                ExcludeRulesWithoutSearchConditions = true,
            };

            var rules = await this.rulesEngine.SearchAsync(searchArgs).ConfigureAwait(false);
            var rqlArray = new RqlArray(rules.Count());
            var i = 0;
            foreach (var rule in rules)
            {
                rqlArray.SetAtIndex(i++, new RqlRule<TContentType, TConditionType>(rule));
            }

            return rqlArray;
        }

        public async Task<object> VisitUnaryExpression(UnaryExpression expression)
        {
            var right = await expression.Right.Accept(this).ConfigureAwait(false);
            if (right is RqlInteger rqlInteger)
            {
                return new RqlInteger(-rqlInteger.Value);
            }

            if (right is RqlDecimal rqlDecimal)
            {
                return new RqlDecimal(-rqlDecimal.Value);
            }

            var rql = this.reverseRqlBuilder.BuildRql(expression);
            var rightRql = this.reverseRqlBuilder.BuildRql(expression.Right);
            throw CreateRuntimeException(
                rql,
                new[] { $"Unary operator '{expression.Operator.Lexeme}' is not supported for expression '{rightRql}'." },
                expression.BeginPosition,
                expression.EndPosition);
        }

        public async Task<object> VisitUpdatableAttributeExpression(UpdatableAttributeExpression updatableAttributeExpression)
        {
            var updatableAttributeExpressionValue = await updatableAttributeExpression.UpdatableAttribute.Accept(this).ConfigureAwait(false);
            var rule = (Rule<TContentType, TConditionType>)this.runtime.Environment.Get(".rule");
            var rql = (string)this.runtime.Environment.Get(".rql");
            switch (updatableAttributeExpression.Kind)
            {
                case UpdatableAttributeKind.DateEnd:
                    rule.DateEnd = updatableAttributeExpressionValue is RqlNothing
                        ? null
                        : ((RqlDate)updatableAttributeExpressionValue).Value;
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

        public async Task<object> VisitUpdateExpression(UpdateExpression updateExpression)
        {
            var rql = this.reverseRqlBuilder.BuildRql(updateExpression);
            var ruleName = (RqlString)await updateExpression.RuleName.Accept(this).ConfigureAwait(false);
            var contentTypeName = (RqlString)await updateExpression.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName.Value, ignoreCase: true);
            var rules = await this.rulesSource
                .GetRulesFilteredAsync(new GetRulesFilteredArgs<TContentType>
                {
                    Name = ruleName.Value,
                    ContentType = contentType,
                }).ConfigureAwait(false);

            if (!rules.Any())
            {
                throw CreateRuntimeException(
                    rql,
                    new[] { $"No such rule with name '{ruleName.Value}' and content type '{contentTypeName.Value}' was found." },
                    updateExpression.BeginPosition,
                    updateExpression.EndPosition);
            }

            var rule = rules.First();
            using (this.runtime.CreateScope())
            {
                this.runtime.Environment.Define(".rule", rule);
                this.runtime.Environment.Define(".rql", rql);

                var updatableAttributes = updateExpression.UpdatableAttributes;
                var updatableAttributesLength = updatableAttributes.Length;
                for (var i = 0; i < updatableAttributesLength; i++)
                {
                    _ = await updatableAttributes[i].Accept(this).ConfigureAwait(false);
                }
            }

            var ruleUpdateResult = await this.rulesEngine.UpdateRuleAsync(rule).ConfigureAwait(false);
            if (!ruleUpdateResult.IsSuccess)
            {
                throw CreateRuntimeException(rql, ruleUpdateResult.Errors, updateExpression.BeginPosition, updateExpression.EndPosition);
            }

            var result = new RqlArray(1);
            result.SetAtIndex(0, new RqlRule<TContentType, TConditionType>(rule));
            return result;
        }

        public async Task<object> VisitValueConditionExpression(ValueConditionExpression valueConditionExpression)
        {
            var conditionTypeName = (RqlString)await valueConditionExpression.Left.Accept(this).ConfigureAwait(false);
            var conditionType = (TConditionType)Enum.Parse(typeof(TConditionType), conditionTypeName.Value, ignoreCase: true);
            var @operator = (Operators)await valueConditionExpression.Operator.Accept(this).ConfigureAwait(false);
            var rightOperand = (IRuntimeValue)await valueConditionExpression.Right.Accept(this).ConfigureAwait(false);
            var dataType = rightOperand switch
            {
                RqlInteger _ or IEnumerable<RqlInteger> _ => DataTypes.Integer,
                RqlBool _ or IEnumerable<RqlBool> _ => DataTypes.Boolean,
                RqlDecimal _ or IEnumerable<RqlDecimal> _ => DataTypes.Decimal,
                RqlString _ or IEnumerable<RqlString> _ => DataTypes.String,
                _ => throw new NotSupportedException(),
            };
            return new ValueConditionNode<TConditionType>(dataType, conditionType, @operator, rightOperand.RuntimeValue);
        }

        public async Task<IResult> VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
        {
            var rql = this.reverseRqlBuilder.BuildRql(variableDeclarationStatement);
            var variableName = (string)await variableDeclarationStatement.Name.Accept(this).ConfigureAwait(false);
            var assignable = await variableDeclarationStatement.Assignable.Accept(this).ConfigureAwait(false);
            this.runtime.Environment.Define(variableName, assignable);
            return new NothingStatementResult(rql);
        }

        public async Task<object> VisitVariableExpression(VariableExpression variableExpression)
        {
            try
            {
                var variableName = (string)await variableExpression.Name.Accept(this).ConfigureAwait(false);
                return this.runtime.Environment.Get(variableName);
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
                    this.runtime.Dispose();
                    this.runtime = null;
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

        private async Task<RuleOperationResult> AddRuleAsync(CreateExpression createStatement, Rule<TContentType, TConditionType> rule)
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

        private async Task<RuleBuilderResult<TContentType, TConditionType>> CreateRuleAsync(CreateExpression createStatement)
        {
            var ruleName = (RqlString)await createStatement.RuleName.Accept(this).ConfigureAwait(false);
            var contentTypeName = (RqlString)await createStatement.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName.Value, ignoreCase: true);
            var content = (IRuntimeValue)await createStatement.Content.Accept(this).ConfigureAwait(false);
            var dateBegin = (RqlDate)await createStatement.DateBegin.Accept(this).ConfigureAwait(false);
            DateTime? dateEnd = null;
            if (createStatement.DateEnd != null)
            {
                var date = (RqlDate)await createStatement.DateEnd.Accept(this).ConfigureAwait(false);
                dateEnd = date.Value;
            }

            IConditionNode<TConditionType> condition = null;
            if (createStatement.Condition != null)
            {
                condition = (IConditionNode<TConditionType>)await createStatement.Condition.Accept(this).ConfigureAwait(false);
            }

            var ruleBuilder = RuleBuilder.NewRule<TContentType, TConditionType>()
                .WithName(ruleName.Value)
                .WithDatesInterval(dateBegin.Value, dateEnd)
                .WithContent(contentType, content.RuntimeValue);

            if (condition != null)
            {
                ruleBuilder.WithCondition(condition);
            }

            return ruleBuilder.Build();
        }
    }
}