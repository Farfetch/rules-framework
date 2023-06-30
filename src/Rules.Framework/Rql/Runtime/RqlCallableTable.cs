namespace Rules.Framework.Rql.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Rql.Runtime.Types;

    internal class RqlCallableTable : ICallableTable
    {
        private readonly IDictionary<string, LinkedList<ICallable>> table;

        public RqlCallableTable()
        {
            this.table = new Dictionary<string, LinkedList<ICallable>>(StringComparer.Ordinal);
        }

        public void AddCallable(ICallable callable)
        {
            if (callable is null)
            {
                throw new ArgumentNullException(nameof(callable));
            }

            this.AddCallable(string.Empty, callable.Name, callable);
        }

        public void AddCallable(RqlType rqlType, ICallable callable)
        {
            if (rqlType.Name is null or "")
            {
                throw new ArgumentException($"Expected non-empty '{nameof(RqlType)}' as parameter.", nameof(rqlType));
            }

            if (callable is null)
            {
                throw new ArgumentNullException(nameof(callable));
            }

            this.AddCallable(rqlType.Name, callable.Name, callable);
        }

        public ICallable ResolveCallable(RqlString callableName, RqlType[] argumentTypes)
        {
            return this.ResolveCallable(string.Empty, callableName, argumentTypes);
        }

        public ICallable ResolveCallable(RqlType callableInstanceType, RqlString callableName, RqlType[] argumentTypes)
        {
            if (callableInstanceType.Name is null or "")
            {
                throw new ArgumentException($"Expected non-empty '{nameof(RqlType)}' as parameter.", nameof(callableInstanceType));
            }

            return this.ResolveCallable(callableInstanceType.Name, callableName, argumentTypes);
        }

        private static string GetCallableTableKey(RqlString callableSpace, RqlString callableName) => $"{callableSpace.Value}.{callableName.Value}";

        private void AddCallable(RqlString callableSpace, RqlString callableName, ICallable callable)
        {
            var callableTableKey = GetCallableTableKey(callableSpace, callableName);
            if (this.table.TryGetValue(callableTableKey, out var callableOverloads))
            {
                var callableNode = callableOverloads.First;
                do
                {
                    var existentCallable = callableNode.Value;
                    if (existentCallable.Arity < callable.Arity)
                    {
                        continue;
                    }

                    if (existentCallable.Arity > callable.Arity)
                    {
                        break;
                    }

                    var isSame = true;
                    for (int i = 0; i < callable.Parameters.Length; i++)
                    {
                        var newCallableParameterType = callable.Parameters[i].Type;
                        var existentCallableParameterType = existentCallable.Parameters[i].Type;
                        if (!newCallableParameterType.IsAssignableTo(existentCallableParameterType))
                        {
                            isSame = false;
                            break;
                        }
                    }

                    if (isSame)
                    {
                        throw new CallableTableException("Callable already exists.", callableSpace, callableName, callable.Parameters.Select(t => t.Type.Name).ToArray());
                    }
                } while (callableNode.Next != null);

                callableOverloads.AddBefore(callableNode, callable);
            }
            else
            {
                callableOverloads = new LinkedList<ICallable>();
                callableOverloads.AddFirst(callable);
                this.table[callableTableKey] = callableOverloads;
            }
        }

        private ICallable ResolveCallable(
                                            RqlString callableSpace,
            RqlString callableName,
            RqlType[] argumentTypes)
        {
            var callableTableKey = GetCallableTableKey(callableSpace, callableName);
            if (this.table.TryGetValue(callableTableKey, out var callableOverloads))
            {
                var callableNode = callableOverloads.First;
                do
                {
                    var callable = callableNode.Value;
                    for (int i = 0; i < argumentTypes.Length; i++)
                    {
                        var argument = argumentTypes[i];
                        var parameter = callable.Parameters[i].Type;
                        if (!argument.IsAssignableTo(parameter))
                        {
                            break;
                        }

                        return callable;
                    }
                }
                while (callableNode.Next != null);
            }

            throw new CallableTableException("Callable has not been found.", callableSpace, callableName, argumentTypes.Select(t => t.Name).ToArray());
        }
    }
}