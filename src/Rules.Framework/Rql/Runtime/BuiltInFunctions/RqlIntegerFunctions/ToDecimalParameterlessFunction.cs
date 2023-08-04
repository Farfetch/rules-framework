namespace Rules.Framework.Rql.Runtime.BuiltInFunctions.RqlIntegerFunctions
{
    using System;
    using Rules.Framework.Rql.Runtime.Types;

    internal class ToDecimalParameterlessFunction : BuiltInFunctionBase
    {
        public override string Name => "ToDecimal";

        public override Parameter[] Parameters => Array.Empty<Parameter>();

        public override RqlType ReturnType => RqlTypes.Decimal;

        public override IRuntimeValue Call(IRuntimeValue instance, IRuntimeValue[] arguments)
        {
            var rqlInteger = (RqlInteger)instance;
            return new RqlDecimal(rqlInteger.Value);
        }
    }
}