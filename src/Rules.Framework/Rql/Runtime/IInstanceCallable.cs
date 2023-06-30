namespace Rules.Framework.Rql.Runtime
{
    using Rules.Framework.Rql.Runtime.Types;

    internal interface IInstanceCallable
    {
        ICallable[] GetCallables(RqlString name);
    }
}