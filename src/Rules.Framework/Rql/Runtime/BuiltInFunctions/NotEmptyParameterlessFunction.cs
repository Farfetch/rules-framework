namespace Rules.Framework.Rql.Runtime.BuiltInFunctions
{
    using System;
    using Rules.Framework.Rql.Runtime.Types;

    internal class NotEmptyParameterlessFunction : BuiltInFunctionBase
    {
        public override string Name => "NotEmpty";

        public override Parameter[] Parameters => Array.Empty<Parameter>();

        public override RqlType ReturnType => RqlTypes.Bool;

        public override IRuntimeValue Call(IRuntimeValue instance, IRuntimeValue[] arguments)
            => new RqlBool(((RqlArray)instance).Size > 0);
    }
}