namespace Rules.Framework.Rql.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Rules.Framework.Core;
    using Rules.Framework.Rql.Runtime.RuleManipulation;
    using Rules.Framework.Rql.Runtime.Types;
    using Rules.Framework.Source;

    internal class RqlRuntime<TContentType, TConditionType> : IRuntime<TContentType, TConditionType>
    {
        private readonly IRulesEngine<TContentType, TConditionType> rulesEngine;
        private readonly IRulesSource<TContentType, TConditionType> rulesSource;
        private ICallableTable callableTable;
        private bool disposedValue;
        private IEnvironment environment;

        private RqlRuntime(
            ICallableTable callableTable,
            IEnvironment environment,
            IRulesEngine<TContentType, TConditionType> rulesEngine,
            IRulesSource<TContentType, TConditionType> rulesSource)
        {
            this.callableTable = callableTable;
            this.environment = environment;
            this.rulesEngine = rulesEngine;
            this.rulesSource = rulesSource;
        }

        public ICallableTable CallableTable => this.callableTable;

        public IEnvironment Environment => this.environment;

        public static IRuntime<TContentType, TConditionType> Create(
            ICallableTable callableTable,
            IEnvironment environment,
            IRulesEngine<TContentType, TConditionType> rulesEngine,
            IRulesSource<TContentType, TConditionType> rulesSource)
        {
            if (callableTable is null)
            {
                throw new ArgumentNullException(nameof(callableTable));
            }

            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            return new RqlRuntime<TContentType, TConditionType>(callableTable, environment, rulesEngine, rulesSource);
        }

        public async ValueTask<RqlArray> ActivateRuleAsync(TContentType contentType, string ruleName)
        {
            var rules = await this.rulesSource.GetRulesFilteredAsync(new GetRulesFilteredArgs<TContentType>
            {
                ContentType = contentType,
                Name = ruleName,
            }).ConfigureAwait(false);

            if (!rules.Any())
            {
                throw new RuntimeException(
                    $"No such rule with name '{ruleName}' and content type '{contentType}' was found.");
            }

            var rule = rules.First();
            if (rule.Active)
            {
                return new RqlArray(0);
            }

            var ruleActivationResult = await this.rulesEngine.ActivateRuleAsync(rule).ConfigureAwait(false);
            if (!ruleActivationResult.IsSuccess)
            {
                throw new RuntimeException(ruleActivationResult.Errors);
            }

            var result = new RqlArray(1);
            result.SetAtIndex(0, new RqlRule<TContentType, TConditionType>(rule));
            return result;
        }

        public IRuntimeValue ApplyUnary(IRuntimeValue value, Operators @operator)
        {
            if (@operator == Operators.Minus)
            {
                if (value is RqlInteger rqlInteger)
                {
                    return new RqlInteger(-rqlInteger.Value);
                }

                if (value is RqlDecimal rqlDecimal)
                {
                    return new RqlDecimal(-rqlDecimal.Value);
                }
            }

            throw new RuntimeException($"Unary operator {@operator} is not supported for value '{value}'.");
        }

        public RqlNothing Assign(string variableName, IRuntimeValue variableValue)
        {
            try
            {
                this.Environment.Assign(variableName, variableValue);
                return new RqlNothing();
            }
            catch (IllegalRuntimeEnvironmentAccessException ex)
            {
                throw new RuntimeException(ex.Message);
            }
        }

        public IRuntimeValue Call(string callableName, IRuntimeValue instance, IRuntimeValue[] arguments)
        {
            try
            {
                if (instance is not null && instance.Type == RqlTypes.Any)
                {
                    instance = ((RqlAny)instance).Unwrap();
                }

                var callable = instance is null
                    ? this.CallableTable.ResolveCallable(callableName, arguments.Select(a => a.Type).ToArray())
                    : this.CallableTable.ResolveCallable(instance.Type, callableName, arguments.Select(a => a.Type).ToArray());
                if (callable is null)
                {
                    throw new RuntimeException($"'{callableName}' was not recognized as a valid callable.");
                }

                var argumentsLength = arguments.Length;
                if (argumentsLength != callable.Arity)
                {
                    throw new RuntimeException(
                        FormattableString.Invariant($"'{callableName}' expects {callable.Arity} argument(s) but {argumentsLength} were provided."));
                }

                return callable.Call(instance, arguments);
            }
            catch (CallableTableException cte)
            {
                var parameterTypesAsString = cte.CallableParameterTypes.Length > 0
                    ? cte.CallableParameterTypes.Aggregate((p1, p2) => $"{p1}, {p2}")
                    : string.Empty;
                throw new RuntimeException($"{cte.Message} - {cte.CallableSpace}.{cte.CallableName}({parameterTypesAsString})");
            }
        }

        public async ValueTask<RqlArray> CreateRuleAsync(CreateRuleArgs<TContentType, TConditionType> createRuleArgs)
        {
            var dateEnd = createRuleArgs.DateEnd.UnderlyingType == RqlTypes.Date
                ? createRuleArgs.DateEnd.Unwrap<RqlDate>().Value
                : (DateTime?)null;
            var ruleBuilder = RuleBuilder.NewRule<TContentType, TConditionType>()
                .WithName(createRuleArgs.Name)
                .WithDatesInterval(createRuleArgs.DateBegin, dateEnd)
                .WithContent(createRuleArgs.ContentType, createRuleArgs.Content.RuntimeValue);

            if (createRuleArgs.Condition != null)
            {
                ruleBuilder.WithCondition(createRuleArgs.Condition);
            }

            var ruleResult = ruleBuilder.Build();
            if (!ruleResult.IsSuccess)
            {
                throw new RuntimeException(ruleResult.Errors);
            }

            var rule = ruleResult.Rule;
            var priorityOption = createRuleArgs.PriorityOption;
            if (priorityOption.IsEmpty)
            {
                switch (this.rulesEngine.GetPriorityCriteria())
                {
                    case PriorityCriterias.TopmostRuleWins:
                        priorityOption = new PriorityOption("TOP", new RqlNothing());
                        break;

                    case PriorityCriterias.BottommostRuleWins:
                        priorityOption = new PriorityOption("BOTTOM", new RqlNothing());
                        break;
                }
            }

            var ruleAddPriorityOption = priorityOption.Option.Value switch
            {
                "TOP" => RuleAddPriorityOption.AtTop,
                "BOTTOM" => RuleAddPriorityOption.AtBottom,
                "NAME" => RuleAddPriorityOption.ByRuleName((string)priorityOption.Argument.Value),
                "NUMBER" => RuleAddPriorityOption.ByPriorityNumber((int)priorityOption.Argument.Value),
                _ => throw new NotSupportedException($"The option '{priorityOption.Option}' is not supported"),
            };

            var ruleAddResult = await this.rulesEngine.AddRuleAsync(rule, ruleAddPriorityOption).ConfigureAwait(false);
            if (!ruleAddResult.IsSuccess)
            {
                throw new RuntimeException(ruleAddResult.Errors);
            }

            var result = new RqlArray(1);
            result.SetAtIndex(0, new RqlRule<TContentType, TConditionType>(rule));
            return result;
        }

        public IDisposable CreateScope()
        {
            var childEnvironment = this.environment.CreateScopedChildEnvironment();
            return new RqlRuntime<TContentType, TConditionType>(this.callableTable, childEnvironment, this.rulesEngine, this.rulesSource);
        }

        public async ValueTask<RqlArray> DeactivateRuleAsync(TContentType contentType, string ruleName)
        {
            var rules = await this.rulesSource.GetRulesFilteredAsync(new GetRulesFilteredArgs<TContentType>
            {
                ContentType = contentType,
                Name = ruleName,
            }).ConfigureAwait(false);

            if (!rules.Any())
            {
                throw new RuntimeException($"No such rule with name '{ruleName}' and content type '{contentType}' was found.");
            }

            var rule = rules.First();
            if (!rule.Active)
            {
                return new RqlArray(0);
            }

            var ruleDeactivationResult = await this.rulesEngine.DeactivateRuleAsync(rule).ConfigureAwait(false);
            if (!ruleDeactivationResult.IsSuccess)
            {
                throw new RuntimeException(ruleDeactivationResult.Errors);
            }

            var result = new RqlArray(1);
            result.SetAtIndex(0, new RqlRule<TContentType, TConditionType>(rule));
            return result;
        }

        public RqlNothing DeclareVariable(RqlString variableName, IRuntimeValue variableValue)
        {
            this.Environment.Define(variableName, variableValue);
            return new RqlNothing();
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IRuntimeValue Divide(IRuntimeValue leftOperand, IRuntimeValue rightOperand) => leftOperand switch
        {
            RqlInteger left when rightOperand is RqlInteger right => new RqlInteger(left.Value / right.Value),
            RqlInteger when rightOperand is RqlDecimal => throw new RuntimeException($"Expected right operand of type {RqlTypes.Integer.Name} but found {RqlTypes.Decimal.Name}. A conversion via 'ToInteger()' of right operand is possible."),
            RqlInteger => throw new RuntimeException($"Expected right operand of type {RqlTypes.Integer.Name} but found {rightOperand.Type.Name}."),
            RqlDecimal left when rightOperand is RqlDecimal right => new RqlDecimal(left.Value / right.Value),
            RqlDecimal when rightOperand is RqlInteger => throw new RuntimeException($"Expected right operand of type {RqlTypes.Decimal.Name} but found {RqlTypes.Integer.Name}. A conversion via 'ToDecimal()' of right operand is possible."),
            RqlDecimal => throw new RuntimeException($"Expected right operand of type {RqlTypes.Decimal.Name} but found {rightOperand.Type.Name}."),
            _ => throw new RuntimeException($"Cannot divide operand of type {leftOperand.Type.Name}."),
        };

        public RqlAny GetAtIndex(IRuntimeValue indexer, RqlInteger index)
        {
            if (indexer.Type == RqlTypes.Any)
            {
                indexer = ((RqlAny)indexer).Unwrap();
            }

            if (indexer is not IIndexerGet indexerGet)
            {
                var type = indexer.Type == RqlTypes.Any ? ((RqlAny)indexer).UnderlyingType.Name : indexer.Type.Name;
                throw new RuntimeException($"Type '{type}' is not a valid indexer target.");
            }

            if (index < 0 || index >= indexerGet.Size)
            {
                var type = indexer.Type == RqlTypes.Any ? ((RqlAny)indexer).UnderlyingType.Name : indexer.Type.Name;
                throw new RuntimeException($"Index '{index}' is out of bounds for instance of '{type}'.");
            }

            return indexerGet.GetAtIndex(index);
        }

        public RqlAny GetPropertyValue(IRuntimeValue instance, RqlString propertyName)
        {
            if (instance.Type == RqlTypes.Any)
            {
                instance = ((RqlAny)instance).Unwrap();
            }

            if (instance is IPropertyGet propertyGet)
            {
                if (propertyGet.TryGetPropertyValue(propertyName, out var propertyValue))
                {
                    return propertyValue;
                }

                throw new RuntimeException($"Instance of '{nameof(RqlObject)}' does not contain property '{propertyName}'.");
            }

            throw new RuntimeException($"Instance does not contain properties.");
        }

        public IRuntimeValue GetVariableValue(RqlString variableName)
        {
            try
            {
                return (IRuntimeValue)this.Environment.Get(variableName);
            }
            catch (IllegalRuntimeEnvironmentAccessException ex)
            {
                throw new RuntimeException(ex.Message);
            }
        }

        public async ValueTask<RqlArray> MatchRulesAsync(MatchCardinality matchCardinality, TContentType contentType, RqlDate matchDate, IEnumerable<Condition<TConditionType>> conditions)
        {
            if (matchCardinality == MatchCardinality.None)
            {
                throw new ArgumentException("A valid match cardinality must be provided.", nameof(matchCardinality));
            }

            if (matchCardinality == MatchCardinality.One)
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

        public IRuntimeValue Multiply(IRuntimeValue leftOperand, IRuntimeValue rightOperand) => leftOperand switch
        {
            RqlInteger left when rightOperand is RqlInteger right => new RqlInteger(left.Value * right.Value),
            RqlInteger when rightOperand is RqlDecimal right => throw new RuntimeException($"Expected right operand of type {RqlTypes.Integer.Name} but found {RqlTypes.Decimal.Name}. A conversion via 'ToInteger()' of right operand is possible."),
            RqlInteger => throw new RuntimeException($"Expected right operand of type {RqlTypes.Integer.Name} but found {rightOperand.Type.Name}."),
            RqlDecimal left when rightOperand is RqlDecimal right => new RqlDecimal(left.Value * right.Value),
            RqlDecimal when rightOperand is RqlInteger => throw new RuntimeException($"Expected right operand of type {RqlTypes.Decimal.Name} but found {RqlTypes.Integer.Name}. A conversion via 'ToDecimal()' of right operand is possible."),
            RqlDecimal => throw new RuntimeException($"Expected right operand of type {RqlTypes.Decimal.Name} but found {rightOperand.Type.Name}."),
            _ => throw new RuntimeException($"Cannot multiply operand of type {leftOperand.Type.Name}."),
        };

        public async ValueTask<RqlArray> SearchRulesAsync(TContentType contentType, RqlDate dateBegin, RqlDate dateEnd, SearchArgs<TContentType, TConditionType> searchArgs)
        {
            var rules = await this.rulesEngine.SearchAsync(searchArgs).ConfigureAwait(false);
            var rqlArray = new RqlArray(rules.Count());
            var i = 0;
            foreach (var rule in rules)
            {
                rqlArray.SetAtIndex(i++, new RqlRule<TContentType, TConditionType>(rule));
            }

            return rqlArray;
        }

        public RqlNothing SetAtIndex(IRuntimeValue indexer, RqlInteger index, IRuntimeValue value)
        {
            if (indexer.Type == RqlTypes.Any)
            {
                indexer = ((RqlAny)indexer).Unwrap();
            }

            if (indexer is not IIndexerSet indexerSet)
            {
                var type = indexer.Type == RqlTypes.Any ? ((RqlAny)indexer).UnderlyingType.Name : indexer.Type.Name;
                throw new RuntimeException($"Type '{type}' is not a valid indexer target.");
            }

            if (index < 0 || index >= indexerSet.Size)
            {
                var type = indexer.Type == RqlTypes.Any ? ((RqlAny)indexer).UnderlyingType.Name : indexer.Type.Name;
                throw new RuntimeException($"Index '{index}' is out of bounds for instance of '{type}'.");
            }

            return indexerSet.SetAtIndex(index, new RqlAny(value));
        }

        public RqlNothing SetPropertyValue(IRuntimeValue instance, RqlString propertyName, IRuntimeValue propertyValue)
        {
            if (instance.Type == RqlTypes.Any)
            {
                instance = ((RqlAny)instance).Unwrap();
            }

            if (instance is IPropertySet propertySet)
            {
                propertySet.SetPropertyValue(propertyName, new RqlAny(propertyValue));
                return new RqlNothing();
            }

            throw new RuntimeException($"Instance of does not contain properties.");
        }

        public async ValueTask<RqlArray> UpdateRuleAsync(UpdateRuleArgs<TContentType> args)
        {
            var rules = await this.rulesSource
                .GetRulesFilteredAsync(new GetRulesFilteredArgs<TContentType>
                {
                    Name = args.Name,
                    ContentType = args.ContentType,
                }).ConfigureAwait(false);

            if (!rules.Any())
            {
                throw new RuntimeException($"No such rule with name '{args.Name.Value}' and content type '{args.ContentType}' was found.");
            }

            var rule = rules.First();
            var updatableAttributes = args.Attributes;
            var updatableAttributesLength = updatableAttributes.Length;
            for (var i = 0; i < updatableAttributesLength; i++)
            {
                switch (updatableAttributes[i].Type)
                {
                    case UpdateRuleAttributeType.DateEnd:
                        await this.UpdateRuleDateEndAsync(rule, (UpdateRuleDateEnd)updatableAttributes[i]).ConfigureAwait(false);
                        break;

                    case UpdateRuleAttributeType.Priority:
                        await this.UpdateRulePriorityAsync(rule, (UpdateRulePriority)updatableAttributes[i]).ConfigureAwait(false);
                        break;

                    case UpdateRuleAttributeType.None:
                    default:
                        throw new NotSupportedException($"The attribute type '{updatableAttributes[i].Type}' is not supported for update.");
                }
            }

            var ruleUpdateResult = await this.rulesEngine.UpdateRuleAsync(rule).ConfigureAwait(false);
            if (!ruleUpdateResult.IsSuccess)
            {
                throw new RuntimeException(ruleUpdateResult.Errors);
            }

            var result = new RqlArray(1);
            result.SetAtIndex(0, new RqlRule<TContentType, TConditionType>(rule));
            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Environment.Dispose();
                }

                disposedValue = true;
            }
        }

        private void DestroyScope()
        {
            if (this.Environment.Parent is not null)
            {
                this.environment = this.Environment.Parent;
            }
        }

        private Task UpdateRuleDateEndAsync(Rule<TContentType, TConditionType> rule, UpdateRuleDateEnd updateRuleDateEnd)
        {
            rule.DateEnd = updateRuleDateEnd.DateEnd.UnderlyingType == RqlTypes.Nothing
                        ? null
                        : (DateTime)updateRuleDateEnd.DateEnd.Value;

            return Task.CompletedTask;
        }

        private async ValueTask UpdateRulePriorityAsync(Rule<TContentType, TConditionType> rule, UpdateRulePriority updateRulePriority)
        {
            var contentType = rule.ContentContainer.ContentType;
            var allContentTypeRulesLazy = new Lazy<Task<IEnumerable<Rule<TContentType, TConditionType>>>>(async () =>
                await this.rulesSource.GetRulesAsync(new GetRulesArgs<TContentType>
                {
                    ContentType = contentType,
                    DateBegin = DateTime.MinValue,
                    DateEnd = DateTime.MaxValue,
                }).ConfigureAwait(false));
            switch (updateRulePriority.Option)
            {
                case "TOP":
                    rule.Priority = 1;
                    break;

                case "BOTTOM":
                    var allRules1 = await allContentTypeRulesLazy.Value.ConfigureAwait(false);
                    rule.Priority = allRules1.Max(r => r.Priority);
                    break;

                case "NUMBER":
                    rule.Priority = updateRulePriority.Argument.Unwrap<RqlInteger>();
                    break;

                case "NAME":
                    var allRules2 = await allContentTypeRulesLazy.Value.ConfigureAwait(false);
                    var argument = updateRulePriority.Argument.Unwrap<RqlString>();
                    var targetPriorityRule = allRules2.FirstOrDefault(r => string.Equals(r.Name, argument, StringComparison.Ordinal));
                    if (targetPriorityRule is null)
                    {
                        throw new RuntimeException($"No such rule with name '{argument.Value}' and content type '{contentType}' was found for target priority.");
                    }

                    rule.Priority = targetPriorityRule.Priority;
                    break;

                default:
                    throw new NotSupportedException($"The priority option '{updateRulePriority.Option}' is not supported.");
            }
        }

        private class RqlRuntimeScope : IDisposable
        {
            private readonly RqlRuntime<TContentType, TConditionType> runtime;

            public RqlRuntimeScope(RqlRuntime<TContentType, TConditionType> runtime)
            {
                this.runtime = runtime;
            }

            public void Dispose() => this.runtime.DestroyScope();
        }
    }
}