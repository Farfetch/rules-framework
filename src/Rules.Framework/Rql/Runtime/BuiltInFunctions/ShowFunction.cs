namespace Rules.Framework.Rql.Runtime.BuiltInFunctions
{
    using System;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Types;

    internal class ShowFunction : BuiltInFunctionBase
    {
        public override string Name => "SHOW";

        public override Parameter[] Parameters => new[] { new Parameter(RqlTypes.String, "value") };

        public override RqlType ReturnType => RqlTypes.Nothing;

        public override object Call(IInterpreter interpreter, object instance, object[] arguments)
        {
            Console.WriteLine(arguments[0].ToString());
            return new RqlNothing();
        }
    }
}