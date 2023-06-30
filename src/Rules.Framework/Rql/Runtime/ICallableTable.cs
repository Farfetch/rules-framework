namespace Rules.Framework.Rql.Runtime
{
    using Rules.Framework.Rql.Runtime.Types;

    internal interface ICallableTable
    {
        ICallable ResolveCallable(RqlString callableName, RqlType[] argumentTypes);

        ICallable ResolveCallable(RqlType callableInstanceType, RqlString callableName, RqlType[] argumentTypes);
    }
}