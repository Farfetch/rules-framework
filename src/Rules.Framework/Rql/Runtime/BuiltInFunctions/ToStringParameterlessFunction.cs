namespace Rules.Framework.Rql.Runtime.BuiltInFunctions
{
    using System;
    using Rules.Framework.Rql.Runtime.Types;

    internal class ToStringParameterlessFunction : BuiltInFunctionBase
    {
        public override string Name => "ToString";

        public override Parameter[] Parameters => Array.Empty<Parameter>();

        public override RqlType ReturnType => RqlTypes.String;

        public override IRuntimeValue Call(IRuntimeValue instance, IRuntimeValue[] arguments)
        {
            var runtimeValue = instance;
            if (runtimeValue is RqlAny rqlAny)
            {
                runtimeValue = rqlAny.Unwrap();
            }
            return runtimeValue switch
            {
                RqlNothing => new RqlString(string.Empty),
                RqlObject rqlObject => new RqlString(rqlObject.ToString(0)),
                RqlReadOnlyObject rqlReadOnlyObject => new RqlString(rqlReadOnlyObject.ToString(0)),
                RqlArray rqlArray => new RqlString(rqlArray.ToString(0)),
                _ => new RqlString(runtimeValue.RuntimeValue.ToString()),
            };
        }
    }
}