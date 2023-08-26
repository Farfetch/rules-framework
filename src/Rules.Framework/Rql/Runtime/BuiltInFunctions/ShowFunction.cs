namespace Rules.Framework.Rql.Runtime.BuiltInFunctions
{
    using System.IO;
    using Rules.Framework.Rql.Runtime.Types;

    internal class ShowFunction : BuiltInFunctionBase
    {
        private readonly TextWriter outputWriter;

        public ShowFunction(TextWriter outputWriter)
        {
            this.outputWriter = outputWriter;
        }

        public override string Name => "Show";

        public override Parameter[] Parameters => new[] { new Parameter(RqlTypes.Any, "value") };

        public override RqlType ReturnType => RqlTypes.Nothing;

        public override IRuntimeValue Call(IRuntimeValue instance, IRuntimeValue[] arguments)
        {
            this.outputWriter.WriteLine(arguments[0].ToString());
            return new RqlNothing();
        }
    }
}