namespace Rules.Framework.Rql.Runtime
{
    using Rules.Framework.Rql.Runtime.Types;

    internal interface ICallable
    {
        int Arity { get; }

        string Name { get; }

        Parameter[] Parameters { get; }

        RqlType ReturnType { get; }

        IRuntimeValue Call(IRuntimeValue instance, IRuntimeValue[] arguments);
    }
}