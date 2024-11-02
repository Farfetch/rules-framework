namespace Rules.Framework.Rql.Runtime
{
    using System.Threading.Tasks;
    using Rules.Framework.Rql.Runtime.Types;

    internal interface IRuntime
    {
        IRuntimeValue ApplyBinary(IRuntimeValue leftOperand, RqlOperators rqlOperator, IRuntimeValue rightOperand);

        IRuntimeValue ApplyUnary(IRuntimeValue value, RqlOperators rqlOperator);

        ValueTask<RqlArray> GetRulesetsAsync();

        ValueTask<RqlArray> MatchRulesAsync(MatchRulesArgs matchRulesArgs);

        ValueTask<RqlArray> SearchRulesAsync(SearchRulesArgs searchRulesArgs);
    }
}