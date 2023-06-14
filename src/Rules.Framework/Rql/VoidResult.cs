namespace Rules.Framework.Rql
{
    public class VoidResult : IResult
    {
        public VoidResult(string rql)
        {
            this.Rql = rql;
        }

        public string Rql { get; }
    }
}