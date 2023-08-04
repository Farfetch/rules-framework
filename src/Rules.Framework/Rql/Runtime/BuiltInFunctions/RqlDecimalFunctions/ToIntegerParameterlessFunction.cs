namespace Rules.Framework.Rql.Runtime.BuiltInFunctions.RqlDecimalFunctions
{
    using System;
    using Rules.Framework.Rql.Runtime.Types;

    internal class ToIntegerParameterlessFunction : BuiltInFunctionBase
    {
        public override string Name => "ToInteger";

        public override Parameter[] Parameters => Array.Empty<Parameter>();

        public override RqlType ReturnType => RqlTypes.Integer;

        public override IRuntimeValue Call(IRuntimeValue instance, IRuntimeValue[] arguments)
        {
            var rqlDecimal = (RqlDecimal)instance;
            var val = decimal.Round(rqlDecimal.Value);
            return new RqlInteger((int)val);
        }
    }
}