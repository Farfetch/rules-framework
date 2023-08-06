namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System;
    using Rules.Framework.Core;
    using Rules.Framework.Rql.Runtime;

    internal static class RqlOperatorExtensions
    {
        public static Operators ToCoreOperator(this RqlOperators rqlOperator) => rqlOperator switch
        {
            RqlOperators.Equals => Operators.Equal,
            RqlOperators.GreaterThan => Operators.GreaterThan,
            RqlOperators.GreaterThanOrEquals => Operators.GreaterThanOrEqual,
            RqlOperators.In => Operators.In,
            RqlOperators.LesserThan => Operators.LesserThan,
            RqlOperators.LesserThanOrEquals => Operators.LesserThanOrEqual,
            RqlOperators.NotEquals => Operators.NotEqual,
            RqlOperators.NotIn => Operators.NotIn,
            _ => throw new NotSupportedException($"Conversion of '{nameof(RqlOperators)}' is not supported: '{rqlOperator}' has no match for '{nameof(Operators)}'."),
        };
    }
}