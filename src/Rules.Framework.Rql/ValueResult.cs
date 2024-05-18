namespace Rules.Framework.Rql
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ValueResult : IResult
    {
        public ValueResult(string rql, object value)
        {
            this.Rql = rql;
            this.Value = value;
        }

        public string Rql { get; }
        public object Value { get; }
    }
}