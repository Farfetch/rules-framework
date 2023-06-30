namespace Rules.Framework.Rql.Runtime
{
    using System;

    internal class RqlRuntime : IRuntime
    {
        private ICallableTable callableTable;
        private bool disposedValue;
        private IEnvironment environment;

        private RqlRuntime(ICallableTable callableTable, IEnvironment environment)
        {
            this.callableTable = callableTable;
            this.environment = environment;
        }

        public ICallableTable CallableTable => this.callableTable;

        public IEnvironment Environment => this.environment;

        public static IRuntime Create(ICallableTable callableTable, IEnvironment environment)
        {
            if (callableTable is null)
            {
                throw new ArgumentNullException(nameof(callableTable));
            }

            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            return new RqlRuntime(callableTable, environment);
        }

        public IDisposable CreateScope()
        {
            var childEnvironment = this.environment.CreateScopedChildEnvironment();
            return new RqlRuntime(this.callableTable, childEnvironment);
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Environment.Dispose();
                }

                disposedValue = true;
            }
        }

        private void DestroyScope()
        {
            if (this.Environment.Parent is not null)
            {
                this.environment = this.Environment.Parent;
            }
        }

        private class RqlRuntimeScope : IDisposable
        {
            private readonly RqlRuntime runtime;

            public RqlRuntimeScope(RqlRuntime runtime)
            {
                this.runtime = runtime;
            }

            public void Dispose() => this.runtime.DestroyScope();
        }
    }
}