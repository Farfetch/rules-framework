namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System.Collections.Generic;

    internal class InterpretResult
    {
        private readonly List<IResult> results;

        public InterpretResult()
        {
            this.results = new List<IResult>();
            this.Success = true;
        }

        public IEnumerable<IResult> Results => this.results.AsReadOnly();

        public bool Success { get; private set; }

        public void AddStatementResult(IResult result)
        {
            this.results.Add(result);

            this.Success &= result.Success;
        }
    }
}