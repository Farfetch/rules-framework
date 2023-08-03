namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
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
                catch (InterpreterException ie)
                {
                    var errorStatementResult = new ErrorStatementResult(ie.Message, ie.Rql, ie.BeginPosition, ie.EndPosition);
                    interpretResult.AddStatementResult(errorStatementResult);
                    break;
                }
            }

            return interpretResult;
        }

        public async Task<IRuntimeValue> VisitActivationExpression(ActivationExpression activationExpression)
        {
            try
            {
                var ruleName = (RqlString)await activationExpression.RuleName.Accept(this).ConfigureAwait(false);
                var contentTypeName = (RqlString)await activationExpression.ContentType.Accept(this).ConfigureAwait(false);
                var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName.Value, ignoreCase: true);
                return await this.runtime.ActivateRuleAsync(contentType, ruleName).ConfigureAwait(false);
            }
            catch (RuntimeException re)
            {
                throw CreateInterpreterException(re.Errors, activationExpression);
            }
        }

        public async Task<IRuntimeValue> VisitAssignExpression(AssignmentExpression assignmentExpression)
        {
            try
            {
                var variableName = (RqlString)await assignmentExpression.Left.Accept(this).ConfigureAwait(false);
                var value = await assignmentExpression.Right.Accept(this).ConfigureAwait(false);
                return this.runtime.Assign(variableName, value);
            }
            catch (RuntimeException ex)
            {
                throw CreateInterpreterException(ex.Message, assignmentExpression);
            }
        }

        public async Task<IResult> VisitBlockStatement(BlockStatement blockStatement)
        {
            var rql = this.reverseRqlBuilder.BuildRql(blockStatement);
            IResult result = null!;
            var statements = blockStatement.Statements;
            var statementsLength = statements.Length;
            if (statementsLength > 0)
            {
                for (int i = 0; i < statementsLength; i++)
                {
                    result = await statements[i].Accept(this).ConfigureAwait(false);
                    if (!result.Success)
                    {
                        return result;
                    }
                }

                return result;
            }

            return new ExpressionStatementResult(rql, new RqlNothing());
        }

        public async Task<IRuntimeValue> VisitCallExpression(CallExpression callExpression)
        {
            try
            {
                var caller = await callExpression.Instance.Accept(this).ConfigureAwait(false);

                string callableName = (RqlString)await callExpression.Name.Accept(this).ConfigureAwait(false);
                int argumentsLength = callExpression.Arguments.Length;
                var arguments = new IRuntimeValue[argumentsLength];
                for (int i = 0; i < argumentsLength; i++)
                {
                    arguments[i] = await callExpression.Arguments[i].Accept(this).ConfigureAwait(false);
                }

                if (caller is RqlNothing && callExpression.Instance == Expression.None)
                {
                    caller = null;
                }

                return this.runtime.Call(callableName, caller!, arguments);
            }
            catch (RuntimeException ex)
            {
                throw CreateInterpreterException(ex.Message, callExpression);
            }
        }

        public async Task<object> VisitCardinalitySegment(CardinalitySegment expression) => await expression.CardinalityKeyword.Accept(this).ConfigureAwait(false);

        public async Task<object> VisitComposedConditionSegment(ComposedConditionSegment expression)
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

        public async Task<object> VisitConditionGroupingSegment(ConditionGroupingSegment expression)
            => await expression.RootCondition.Accept(this).ConfigureAwait(false);

        public async Task<IRuntimeValue> VisitCreateExpression(CreateExpression createExpression)
        {
            var ruleName = (RqlString)await createExpression.RuleName.Accept(this).ConfigureAwait(false);
            var contentTypeName = (RqlString)await createExpression.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName.Value, ignoreCase: true);
            var content = await createExpression.Content.Accept(this).ConfigureAwait(false);
            var dateBegin = (RqlDate)await createExpression.DateBegin.Accept(this).ConfigureAwait(false);
            RqlAny dateEnd = new RqlNothing();
            if (createExpression.DateEnd != null)
            {
                dateEnd = (RqlDate)await createExpression.DateEnd.Accept(this).ConfigureAwait(false);
            }

            IConditionNode<TConditionType> condition = null!;
            if (createExpression.Condition != null)
            {
                condition = (IConditionNode<TConditionType>)await createExpression.Condition.Accept(this).ConfigureAwait(false);
            }

            PriorityOption priorityOption = PriorityOption.None;
            if (createExpression.PriorityOption != null)
            {
                priorityOption = (PriorityOption)await createExpression.PriorityOption.Accept(this).ConfigureAwait(false);
            }

            var createRuleArgs = CreateRuleArgs<TContentType, TConditionType>.Create(contentType, ruleName, content, dateBegin, dateEnd, condition, priorityOption);
            return await this.runtime.CreateRuleAsync(createRuleArgs).ConfigureAwait(false);
        }

        public async Task<object> VisitDateEndSegment(DateEndSegment dateEndSegment)
            => await dateEndSegment.DateEnd.Accept(this).ConfigureAwait(false);

        public async Task<IRuntimeValue> VisitDeactivationExpression(DeactivationExpression deactivationExpression)
        {
            var ruleName = (RqlString)await deactivationExpression.RuleName.Accept(this).ConfigureAwait(false);
            var contentTypeName = (RqlString)await deactivationExpression.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName.Value, ignoreCase: true);
            return await this.runtime.DeactivateRuleAsync(contentType, ruleName).ConfigureAwait(false);
        }

        public async Task<IResult> VisitExpressionStatement(ExpressionStatement expressionStatement)
        {
            var rql = this.reverseRqlBuilder.BuildRql(expressionStatement);
            var expressionResult = await expressionStatement.Expression.Accept(this).ConfigureAwait(false);
            return new ExpressionStatementResult(rql, expressionResult);
        }

        public async Task<IResult> VisitForEachStatement(ForEachStatement forEachStatement)
        {
            var rql = this.reverseRqlBuilder.BuildRql(forEachStatement);
            var source = await forEachStatement.SourceExpression.Accept(this).ConfigureAwait(false);
            if (source.Type == RqlTypes.Any)
            {
                source = ((RqlAny)source).Unwrap();
            }

            if (source.Type != RqlTypes.Array)
            {
                throw CreateInterpreterException("Expected array value for 'foreach' source.", forEachStatement);
            }

            var sourceArray = (RqlArray)source;
            var length = sourceArray.Size;
            using (var scope = this.runtime.CreateScope())
            {
                _ = await forEachStatement.VariableDeclaration.Accept(this).ConfigureAwait(false);
                var variableName = (RqlString)await ((VariableDeclarationExpression)forEachStatement.VariableDeclaration).Name.Accept(this).ConfigureAwait(false);
                for (int i = 0; i < length; i++)
                {
                    this.runtime.Assign(variableName, sourceArray.GetAtIndex(i));
                    _ = await forEachStatement.ForEachActionStatement.Accept(this).ConfigureAwait(false);
                }
            }

            return new NothingStatementResult(rql);
        }

        public Task<IRuntimeValue> VisitIdentifierExpression(IdentifierExpression identifierExpression)
            => Task.FromResult<IRuntimeValue>(new RqlString(identifierExpression.Identifier.UnescapedLexeme));

        public async Task<IResult> VisitIfStatement(IfStatement ifStatement)
        {
            var condition = await ifStatement.Condition.Accept(this).ConfigureAwait(false);
            if (condition.Type == RqlTypes.Any)
            {
                condition = ((RqlAny)condition).Unwrap();
            }

            if (condition.Type != RqlTypes.Bool)
            {
                throw CreateInterpreterException("Expected bool value for 'if' condition.", ifStatement);
            }

            if ((RqlBool)condition)
            {
                return await ifStatement.ThenBranch.Accept(this).ConfigureAwait(false);
            }

            return await ifStatement.ElseBranch.Accept(this).ConfigureAwait(false);
        }

        public async Task<IRuntimeValue> VisitIndexerGetExpression(IndexerGetExpression indexerGetExpression)
        {
            try
            {
                var instance = await indexerGetExpression.Instance.Accept(this).ConfigureAwait(false);
                var index = (RqlInteger)await indexerGetExpression.Index.Accept(this).ConfigureAwait(false);
                return this.runtime.GetAtIndex(instance, index);
            }
            catch (RuntimeException re)
            {
                throw CreateInterpreterException(re.Errors, indexerGetExpression);
            }
        }

        public async Task<IRuntimeValue> VisitIndexerSetExpression(IndexerSetExpression indexerSetExpression)
        {
            try
            {
                var instance = await indexerSetExpression.Instance.Accept(this).ConfigureAwait(false);
                var index = (RqlInteger)await indexerSetExpression.Index.Accept(this).ConfigureAwait(false);
                var value = await indexerSetExpression.Value.Accept(this).ConfigureAwait(false);
                return this.runtime.SetAtIndex(instance, index, value);
            }
            catch (RuntimeException re)
            {
                throw CreateInterpreterException(re.Errors, indexerSetExpression);
            }
        }

        public async Task<object> VisitInputConditionSegment(InputConditionSegment inputConditionExpression)
        {
            var conditionTypeName = (RqlString)await inputConditionExpression.Left.Accept(this).ConfigureAwait(false);
            if (!Enum.TryParse(typeof(TConditionType), conditionTypeName.Value, out var conditionType))
            {
                throw CreateInterpreterException(new[] { FormattableString.Invariant($"Condition type of name '{conditionTypeName}' was not found.") }, inputConditionExpression);
            }

            var conditionValue = await inputConditionExpression.Right.Accept(this).ConfigureAwait(false);
            return new Condition<TConditionType>((TConditionType)conditionType, conditionValue.RuntimeValue);
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
            var cardinality = (RqlString)await matchExpression.Cardinality.Accept(this).ConfigureAwait(false);
            var contentTypeName = (RqlString)await matchExpression.ContentType.Accept(this).ConfigureAwait(false);
            var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName.Value, ignoreCase: true);
            var matchDate = (RqlDate)await matchExpression.MatchDate.Accept(this).ConfigureAwait(false);
            var inputConditions = await matchExpression.InputConditions.Accept(this).ConfigureAwait(false);
            var conditions = inputConditions is null ? Array.Empty<Condition<TConditionType>>() : (IEnumerable<Condition<TConditionType>>)inputConditions;
            var matchCardinality = string.Equals(cardinality.Value, "ONE", StringComparison.OrdinalIgnoreCase)
                ? MatchCardinality.One
                : MatchCardinality.All;

            return await this.runtime.MatchRulesAsync(matchCardinality, contentType, matchDate, conditions).ConfigureAwait(false);
        }

        public async Task<IRuntimeValue> VisitNewArrayExpression(NewArrayExpression newArrayExpression)
        {
            var sizeValue = await newArrayExpression.Size.Accept(this).ConfigureAwait(false);
            var size = sizeValue is RqlInteger integer ? integer.Value : newArrayExpression.Values.Length;
            var rqlArray = new RqlArray(size);

            if (newArrayExpression.Values.Length > 0)
            {
                for (var i = 0; i < size; i++)
                {
                    var value = await newArrayExpression.Values[i].Accept(this).ConfigureAwait(false);
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

        public Task<object> VisitNoneSegment(NoneSegment noneSegment) => Task.FromResult<object>(new RqlNothing());

        public Task<IResult> VisitNoneStatement(NoneStatement statement) => Task.FromResult<IResult>(new ExpressionStatementResult(string.Empty, new RqlNothing()));

        public Task<object> VisitOperatorSegment(OperatorSegment operatorExpression)
            => Task.FromResult<object>(result: operatorExpression.Token.Type switch
            {
                TokenType.EQUAL => Core.Operators.Equal,
                TokenType.NOT_EQUAL => Core.Operators.NotEqual,
                TokenType.GREATER_THAN => Core.Operators.GreaterThan,
                TokenType.GREATER_THAN_OR_EQUAL => Core.Operators.GreaterThanOrEqual,
                TokenType.LESS_THAN => Core.Operators.LesserThan,
                TokenType.LESS_THAN_OR_EQUAL => Core.Operators.LesserThanOrEqual,
                TokenType.IN => Core.Operators.In,
                TokenType.NOT_IN => Core.Operators.NotIn,
                _ => throw new NotSupportedException($"The token type '{operatorExpression.Token.Type}' is not supported as valid operator."),
            });

        public Task<IRuntimeValue> VisitPlaceholderExpression(PlaceholderExpression placeholderExpression)
            => Task.FromResult<IRuntimeValue>(new RqlString((string)placeholderExpression.Token.Literal));

        public async Task<object> VisitPriorityOptionSegment(PriorityOptionSegment priorityOptionExpression)
        {
            var option = (RqlString)await priorityOptionExpression.PriorityOption.Accept(this).ConfigureAwait(false);
            var optionAsUppercase = option.Value.ToUpperInvariant();
            switch (optionAsUppercase)
            {
                case "TOP":
                    return new PriorityOption(optionAsUppercase, new RqlNothing());

                case "BOTTOM":
                    return new PriorityOption(optionAsUppercase, new RqlNothing());

                case "NAME":
                    RqlString ruleName = (RqlString)await priorityOptionExpression.Argument.Accept(this).ConfigureAwait(false);
                    return new PriorityOption(optionAsUppercase, ruleName);

                case "NUMBER":
                    RqlInteger priorityNumber = (RqlInteger)await priorityOptionExpression.Argument.Accept(this).ConfigureAwait(false);
                    return new PriorityOption(optionAsUppercase, priorityNumber);

                default:
                    throw new NotSupportedException($"The option '{option}' is not supported");
            }
        }

        public async Task<IRuntimeValue> VisitPropertyGetExpression(PropertyGetExpression propertyGetExpression)
        {
            try
            {
                var instance = await propertyGetExpression.Instance.Accept(this).ConfigureAwait(false);
                var propertyName = (RqlString)await propertyGetExpression.Name.Accept(this).ConfigureAwait(false);
                return this.runtime.GetPropertyValue(instance, propertyName);
            }
            catch (RuntimeException re)
            {
                throw CreateInterpreterException(re.Errors, propertyGetExpression);
            }
        }

        public async Task<IRuntimeValue> VisitPropertySetExpression(PropertySetExpression propertySetExpression)
        {
            try
            {
                var instance = await propertySetExpression.Instance.Accept(this).ConfigureAwait(false);
                var propertyName = (RqlString)await propertySetExpression.Name.Accept(this).ConfigureAwait(false);
                var value = await propertySetExpression.Value.Accept(this).ConfigureAwait(false);
                return this.runtime.SetPropertyValue(instance, propertyName, value);
            }
            catch (RuntimeException re)
            {
                throw CreateInterpreterException(re.Errors, propertySetExpression);
            }
        }

        public async Task<IRuntimeValue> VisitSearchExpression(SearchExpression searchExpression)
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

            return await this.runtime.SearchRulesAsync(contentType, dateBegin, dateEnd, searchArgs).ConfigureAwait(false);
        }

        public async Task<IRuntimeValue> VisitUnaryExpression(UnaryExpression unaryExpression)
        {
            try
            {
                var @operator = unaryExpression.Operator.Lexeme switch
                {
                    "-" => Runtime.Operators.Minus,
                    _ => Runtime.Operators.None,
                };
                var right = await unaryExpression.Right.Accept(this).ConfigureAwait(false);
                return this.runtime.ApplyUnary(right, @operator);
            }
            catch (RuntimeException re)
            {
                throw CreateInterpreterException(re.Errors, unaryExpression);
            }
        }

        public async Task<object> VisitUpdatableAttributeSegment(UpdatableAttributeSegment updatableAttributeExpression)
        {
            var updatableAttributeExpressionValue = await updatableAttributeExpression.UpdatableAttribute.Accept(this).ConfigureAwait(false);
            switch (updatableAttributeExpression.Kind)
            {
                case UpdatableAttributeKind.DateEnd:
                    return UpdateRuleDateEnd.Create((RqlDate)updatableAttributeExpressionValue);

                case UpdatableAttributeKind.PriorityOption:
                    var priorityOption = (PriorityOption)updatableAttributeExpressionValue;
                    return UpdateRulePriority.Create(priorityOption.Option, priorityOption.Argument);

                default:
                    throw new NotSupportedException($"The updatable attribute kind '{updatableAttributeExpression.Kind}' is not supported.");
            }
        }

        public async Task<IRuntimeValue> VisitUpdateExpression(UpdateExpression updateExpression)
        {
            try
            {
                var ruleName = (RqlString)await updateExpression.RuleName.Accept(this).ConfigureAwait(false);
                var contentTypeName = (RqlString)await updateExpression.ContentType.Accept(this).ConfigureAwait(false);
                var contentType = (TContentType)Enum.Parse(typeof(TContentType), contentTypeName.Value, ignoreCase: true);
                var updateRuleArgs = new UpdateRuleArgs<TContentType>(contentType, ruleName);
                for (int i = 0; i < updateExpression.UpdatableAttributes.Length; i++)
                {
                    var updateRuleAttribute = (UpdateRuleAttribute)await updateExpression.UpdatableAttributes[i].Accept(this).ConfigureAwait(false);
                    updateRuleArgs.AddAttributeToUpdate(updateRuleAttribute);
                }

                return await this.runtime.UpdateRuleAsync(updateRuleArgs).ConfigureAwait(false);
            }
            catch (RuntimeException re)
            {
                throw CreateInterpreterException(re.Errors, updateExpression);
            }
        }

        public async Task<object> VisitValueConditionSegment(ValueConditionSegment valueConditionExpression)
        {
            var conditionTypeName = (RqlString)await valueConditionExpression.Left.Accept(this).ConfigureAwait(false);
            var conditionType = (TConditionType)Enum.Parse(typeof(TConditionType), conditionTypeName.Value, ignoreCase: true);
            var @operator = (Core.Operators)await valueConditionExpression.Operator.Accept(this).ConfigureAwait(false);
            var rightOperand = await valueConditionExpression.Right.Accept(this).ConfigureAwait(false);
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

        public async Task<IResult> VisitVariableBootstrapStatement(VariableBootstrapStatement variableBootstrapStatement)
        {
            var rql = this.reverseRqlBuilder.BuildRql(variableBootstrapStatement);
            var variableName = (RqlString)await ((VariableDeclarationExpression)variableBootstrapStatement.VariableDeclaration).Name.Accept(this).ConfigureAwait(false);
            var assignable = await variableBootstrapStatement.Assignable.Accept(this).ConfigureAwait(false);
            this.runtime.DeclareVariable(variableName, assignable);
            return new NothingStatementResult(rql);
        }

        public async Task<IRuntimeValue> VisitVariableDeclarationExpression(VariableDeclarationExpression variableDeclarationExpression)
        {
            var variableName = (RqlString)await variableDeclarationExpression.Name.Accept(this).ConfigureAwait(false);
            return this.runtime.DeclareVariable(variableName, new RqlNothing());
        }

        public async Task<IRuntimeValue> VisitVariableExpression(VariableExpression variableExpression)
        {
            try
            {
                var variableName = (RqlString)await variableExpression.Name.Accept(this).ConfigureAwait(false);
                return this.runtime.GetVariableValue(variableName);
            }
            catch (RuntimeException re)
            {
                throw CreateInterpreterException(re.Errors, variableExpression);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.runtime.Dispose();
                    this.runtime = null!;
                }

                disposedValue = true;
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
    }
}