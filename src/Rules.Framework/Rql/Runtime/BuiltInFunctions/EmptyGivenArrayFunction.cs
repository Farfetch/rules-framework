namespace Rules.Framework.Rql.Runtime.BuiltInFunctions
{
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Runtime.Types;

    internal class EmptyGivenArrayFunction : BuiltInFunctionBase
    {
        public override string Name => "Empty";

        public override Parameter[] Parameters => new[]
        {
            new Parameter(RqlTypes.Array, "items"),
        };

        public override RqlType ReturnType => RqlTypes.Bool;

        public override IRuntimeValue Call(IInterpreter interpreter, IRuntimeValue instance, IRuntimeValue[] arguments)
            => new RqlBool(((RqlArray)arguments[0]).Size == 0);
    }
}