namespace Rules.Framework.Generics
{
    public class GenericNothingRqlResult : IGenericRqlResult
    {
        public GenericNothingRqlResult(string rql)
        {
            this.Rql = rql;
        }

        public string Rql { get; }
    }
}