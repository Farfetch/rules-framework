namespace Rules.Framework.Rql.Runtime
{
    using Rules.Framework.Rql.Types;

    internal interface IIndexerSet
    {
        RqlInteger Size { get; }

        RqlNothing SetAtIndex(RqlInteger index, RqlAny value);
    }
}