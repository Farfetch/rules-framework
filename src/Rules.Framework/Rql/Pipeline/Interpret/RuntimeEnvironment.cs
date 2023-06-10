namespace Rules.Framework.Rql.Pipeline.Interpret
{
    using System;
    using System.Collections.Generic;

    internal class RuntimeEnvironment : IRuntimeEnvironment
    {
        private readonly IRuntimeEnvironment parentRuntimeEnvironment;
        private readonly Dictionary<string, object> runtimeData;
        private bool disposedValue;

        public RuntimeEnvironment()
            : this(parentRuntimeEnvironment: null)
        {
        }

        public RuntimeEnvironment(IRuntimeEnvironment parentRuntimeEnvironment)
        {
            this.parentRuntimeEnvironment = parentRuntimeEnvironment;
            this.runtimeData = new Dictionary<string, object>(StringComparer.Ordinal);
        }

        public void AssignVariable(string name, object value)
        {
            if (!this.runtimeData.ContainsKey(name))
            {
                throw new IllegalRuntimeEnvironmentAccessException($"Cannot assign undefined variable '{name}'.", name);
            }

            this.runtimeData[name] = value;
        }

        public void CreateAndAssignVariable(string name, object value)
            => this.runtimeData[name] = value;

        public IRuntimeEnvironment CreateScopedChildRuntimeEnvironment()
            => new RuntimeEnvironment(this);

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public object GetVariableValue(string name)
        {
            if (!this.runtimeData.TryGetValue(name, out var value))
            {
                throw new IllegalRuntimeEnvironmentAccessException($"Cannot get value for undefined variable '{name}'.", name);
            }

            return value;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.runtimeData.Clear();
                }

                disposedValue = true;
            }
        }
    }
}