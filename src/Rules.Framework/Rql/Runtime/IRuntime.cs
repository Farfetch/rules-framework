namespace Rules.Framework.Rql.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Rql.Runtime.RuleManipulation;
    using Rules.Framework.Rql.Runtime.Types;

    internal interface IRuntime<TContentType, TConditionType> : IDisposable
    {
        ICallableTable CallableTable { get; }

        IEnvironment Environment { get; }

        ValueTask<RqlArray> ActivateRuleAsync(TContentType contentType, string ruleName);

        IRuntimeValue ApplyUnary(IRuntimeValue value, Operators @operator);

        RqlNothing Assign(string variableName, IRuntimeValue variableValue);

        IRuntimeValue Call(string callableName, IRuntimeValue instance, IRuntimeValue[] arguments);

        ValueTask<RqlArray> CreateRuleAsync(CreateRuleArgs<TContentType, TConditionType> createRuleArgs);

        IDisposable CreateScope();

        ValueTask<RqlArray> DeactivateRuleAsync(TContentType contentType, string ruleName);

        RqlNothing DeclareVariable(RqlString variableName, IRuntimeValue variableValue);

        IRuntimeValue Divide(IRuntimeValue leftOperand, IRuntimeValue rightOperand);

        RqlAny GetAtIndex(IRuntimeValue indexer, RqlInteger index);

        RqlAny GetPropertyValue(IRuntimeValue instance, RqlString propertyName);

        IRuntimeValue GetVariableValue(RqlString variableName);

        ValueTask<RqlArray> MatchRulesAsync(
            MatchCardinality matchCardinality,
            TContentType contentType,
            RqlDate matchDate,
            IEnumerable<Condition<TConditionType>> conditions);

        IRuntimeValue Multiply(IRuntimeValue leftOperand, IRuntimeValue rightOperand);

        ValueTask<RqlArray> SearchRulesAsync(TContentType contentType, RqlDate dateBegin, RqlDate dateEnd, SearchArgs<TContentType, TConditionType> searchArgs);

        RqlNothing SetAtIndex(IRuntimeValue indexer, RqlInteger index, IRuntimeValue value);

        RqlNothing SetPropertyValue(IRuntimeValue instance, RqlString propertyName, IRuntimeValue propertyValue);

        ValueTask<RqlArray> UpdateRuleAsync(UpdateRuleArgs<TContentType> args);
    }
}