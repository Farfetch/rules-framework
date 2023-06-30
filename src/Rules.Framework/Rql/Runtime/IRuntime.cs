namespace Rules.Framework.Rql.Runtime
{
    using System;

    internal interface IRuntime : IDisposable
    {
        ICallableTable CallableTable { get; }

        IEnvironment Environment { get; }

        IDisposable CreateScope();
    }
}