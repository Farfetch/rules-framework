namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System;

    internal interface IRuntimeEnvironment : IDisposable
    {
        void Assign(string name, object value);

        IRuntimeEnvironment CreateScopedChildRuntimeEnvironment();

        void Define(string name, object value);

        object Get(string name);
    }
}