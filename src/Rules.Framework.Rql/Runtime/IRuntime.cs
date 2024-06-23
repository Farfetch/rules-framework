namespace Rules.Framework.Rql.Runtime
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Rql.Runtime.RuleManipulation;
    using Rules.Framework.Rql.Runtime.Types;

    internal interface IRuntime<TContentType, TConditionType>
    {
        IRuntimeValue ApplyBinary(IRuntimeValue leftOperand, RqlOperators rqlOperator, IRuntimeValue rightOperand);

        IRuntimeValue ApplyUnary(IRuntimeValue value, RqlOperators rqlOperator);

        ValueTask<RqlArray> MatchRulesAsync(MatchRulesArgs<TContentType, TConditionType> matchRulesArgs);

        ValueTask<RqlArray> SearchRulesAsync(SearchRulesArgs<TContentType, TConditionType> searchRulesArgs);
    }
}