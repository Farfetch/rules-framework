namespace Rules.Framework.Rql.Runtime
{
    using System;

    internal interface IEnvironment : IDisposable
    {
        IEnvironment Parent { get; }

        void Assign(string name, object value);

        IEnvironment CreateScopedChildEnvironment();

        void Define(string name, object value);

        object Get(string name);
    }
}