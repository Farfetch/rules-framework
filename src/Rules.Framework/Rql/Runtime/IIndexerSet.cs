namespace Rules.Framework.Rql.Runtime
{
    using Rules.Framework.Rql.Runtime.Types;

    internal interface IIndexerSet
    {
        RqlInteger Size { get; }

        RqlNothing SetAtIndex(RqlInteger index, RqlAny value);
    }
}