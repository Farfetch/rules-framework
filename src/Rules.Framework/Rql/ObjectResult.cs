namespace Rules.Framework.Rql
{
    public class ObjectResult : IResult
    {
        public ObjectResult(string rql, object value)
        {
            this.Rql = rql;
            this.Value = value;
        }

        public string Rql { get; }
        public object Value { get; }
    }
}