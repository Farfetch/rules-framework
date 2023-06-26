namespace Rules.Framework.Rql.Runtime
{
    using Rules.Framework.Rql.Types;

    internal interface IIndexerGet
    {
        RqlInteger Size { get; }

        RqlAny GetAtIndex(RqlInteger index);
    }
}