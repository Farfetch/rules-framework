namespace Rules.Framework.Rql.Pipeline.Interpret.BuiltInFunctions
{
    using System;

    internal class ShowFunction : BuiltInFunctionBase
    {
        public override string Name => "SHOW";

        public override string[] Parameters => new[] { "value" };

        public override object Call(IInterpreter interpreter, object[] arguments)
        {
            Console.WriteLine(arguments[0].ToString());
            return null;
        }
    }
}