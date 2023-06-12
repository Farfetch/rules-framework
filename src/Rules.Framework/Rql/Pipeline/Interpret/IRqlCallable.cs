namespace Rules.Framework.Rql.Pipeline.Interpret
{
    internal interface IRqlCallable
    {
        int Arity { get; }

        string Name { get; }

        object Call(IInterpreter interpreter, object[] arguments);
    }
}