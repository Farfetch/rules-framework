namespace Rules.Framework.Rql
{
    public class NothingResult : IResult
    {
        public NothingResult(string rql)
        {
            this.Rql = rql;
        }

        public string Rql { get; }
    }
}