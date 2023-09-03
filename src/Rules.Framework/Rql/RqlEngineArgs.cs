namespace Rules.Framework.Rql
{
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Pipeline.Parse;
    using Rules.Framework.Rql.Pipeline.Scan;

    internal class RqlEngineArgs<TContentType, TConditionType>
    {
        public IInterpreter Interpreter { get; set; }

        public RqlOptions Options { get; set; }

        public Parser Parser { get; set; }

        public Scanner Scanner { get; set; }
    }
}