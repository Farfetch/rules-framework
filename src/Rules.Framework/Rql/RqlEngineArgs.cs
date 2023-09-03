namespace Rules.Framework.Rql
{
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Pipeline.Parse;
    using Rules.Framework.Rql.Pipeline.Scan;

    internal class RqlEngineArgs
    {
        public IInterpreter Interpreter { get; set; }

        public RqlOptions Options { get; set; }

        public IParser Parser { get; set; }

        public IScanner Scanner { get; set; }
    }
}