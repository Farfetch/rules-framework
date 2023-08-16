namespace Rules.Framework.Generics
{
    public class GenericValueRqlResult : IGenericRqlResult
    {
        public GenericValueRqlResult(string rql, object value)
        {
            this.Rql = rql;
            this.Value = value;
        }

        public string Rql { get; }

        public object Value { get; }
    }
}