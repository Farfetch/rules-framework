namespace Rules.Framework.Rql
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class NothingResult : IResult
    {
        public NothingResult(string rql)
        {
            this.Rql = rql;
        }

        public string Rql { get; }
    }
}