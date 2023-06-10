namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System;

    internal interface IRuntimeEnvironment : IDisposable
    {
        void AssignVariable(string name, object value);

        void CreateAndAssignVariable(string name, object value);

        IRuntimeEnvironment CreateScopedChildRuntimeEnvironment();

        object GetVariableValue(string name);
    }
}