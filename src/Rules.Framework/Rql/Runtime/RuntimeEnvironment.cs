using Rules.Framework.Rql.Pipeline.Interpret;

namespace Rules.Framework.Rql.Runtime
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

        public void Assign(string name, object value)
        {
            if (this.runtimeData.ContainsKey(name))
            {
                this.runtimeData[name] = value;
            }

            if (this.parentRuntimeEnvironment is not null)
            {
                this.parentRuntimeEnvironment.Assign(name, value);
            }

            throw new IllegalRuntimeEnvironmentAccessException($"Cannot assign undefined '{name}'.", name);
        }

        public IRuntimeEnvironment CreateScopedChildRuntimeEnvironment()
            => new RuntimeEnvironment(this);

        public void Define(string name, object value)
                    => this.runtimeData[name] = value;

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public object Get(string name)
        {
            if (this.runtimeData.TryGetValue(name, out var value))
            {
                return value;
            }

            if (this.parentRuntimeEnvironment is not null)
            {
                return this.parentRuntimeEnvironment.Get(name);
            }

            throw new IllegalRuntimeEnvironmentAccessException($"Cannot get value for undefined '{name}'.", name);
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