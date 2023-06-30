namespace Rules.Framework.Rql.Runtime.BuiltInFunctions
{
    using System;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Runtime.Types;

    internal class ShowFunction : BuiltInFunctionBase
    {
        public override string Name => "Show";

        public override Parameter[] Parameters => new[] { new Parameter(RqlTypes.Any, "value") };

        public override RqlType ReturnType => RqlTypes.Nothing;

        public override IRuntimeValue Call(IInterpreter interpreter, IRuntimeValue instance, IRuntimeValue[] arguments)
        {
            Console.WriteLine(arguments[0].ToString());
            return new RqlNothing();
        }
    }
}