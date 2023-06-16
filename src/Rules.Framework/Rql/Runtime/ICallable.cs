namespace Rules.Framework.Rql.Runtime
{
    using Rules.Framework.Rql.Pipeline.Interpret;
    using Rules.Framework.Rql.Types;

    internal interface ICallable
    {
        int Arity { get; }

        string Name { get; }

        Parameter[] Parameters { get; }

        RqlType ReturnType { get; }

        object Call(IInterpreter interpreter, object instance, object[] arguments);
    }
}