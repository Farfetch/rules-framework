namespace Rules.Framework.Rql
{
    using System.Diagnostics.CodeAnalysis;
    using Rules.Framework.Rql.Pipeline.Assist;
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Pipeline.Parse;
    using Rules.Framework.Rql.Pipeline.Scan;

    [ExcludeFromCodeCoverage]
    internal class RqlEngineArgs
    {
        public IAssistEngine AssistEngine { get; set; }

        public IInterpreter Interpreter { get; set; }

        public RqlOptions Options { get; set; }

        public IParser Parser { get; set; }

        public ITokenScanner TokenScanner { get; set; }
    }
}