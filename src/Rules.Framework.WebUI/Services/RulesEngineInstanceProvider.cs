namespace Rules.Framework.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class RulesEngineInstanceProvider : IRulesEngineInstanceProvider, IRulesEngineInstancesRegistrar
    {
        private readonly List<(Guid, string, Func<IServiceProvider, string, IRulesEngine>)> instanceMappings;
        private readonly Dictionary<Guid, RulesEngineInstance> instances;
        private bool instancesEnumerated;

        public RulesEngineInstanceProvider()
        {
            this.instanceMappings = new List<(Guid, string, Func<IServiceProvider, string, IRulesEngine>)>();
            this.instances = new Dictionary<Guid, RulesEngineInstance>();
            this.instancesEnumerated = false;
        }

        public IRulesEngineInstancesRegistrar AddInstance(string name, Func<IServiceProvider, string, IRulesEngine> getRulesEngineFunc)
        {
            if (this.instanceMappings.Any(m => string.Equals(m.Item2, name, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException($"A rules engine instance with name '{name}' has already been specified.");
            }

            this.instanceMappings.Add((GuidGenerator.GenerateFromString(name), name, getRulesEngineFunc));
            return this;
        }

        public IRulesEngineInstancesRegistrar AddInstance(string name, Func<IServiceProvider, string, Task<IRulesEngine>> getRulesEngineFunc)
            => this.AddInstance(name, (serviceProvider, name) => getRulesEngineFunc.Invoke(serviceProvider, name).GetAwaiter().GetResult());

        public void EnumerateInstances(IServiceProvider serviceProvider)
        {
            if (!instancesEnumerated)
            {
                foreach (var (id, name, getRulesEngineFunc) in instanceMappings)
                {
                    var instance = new RulesEngineInstance
                    {
                        Id = id,
                        Name = name,
                        RulesEngine = getRulesEngineFunc.Invoke(serviceProvider, name),
                    };

                    this.instances.Add(id, instance);
                }

                instancesEnumerated = true;
            }
        }

        public IEnumerable<RulesEngineInstance> GetAllInstances()
        {
            if (!instancesEnumerated)
            {
                throw new InvalidOperationException($"Instances enumeration is required before invoking '{nameof(GetAllInstances)}'." +
                    $" Please ensure '{nameof(EnumerateInstances)}' is invoked first.");
            }

            return this.instances.Values;
        }

        public RulesEngineInstance GetInstance(Guid instanceId)
        {
            if (!instancesEnumerated)
            {
                throw new InvalidOperationException($"Instances enumeration is required before invoking '{nameof(GetInstance)}'." +
                    $" Please ensure '{nameof(EnumerateInstances)}' is invoked first.");
            }

            if (this.instances.TryGetValue(instanceId, out var instance))
            {
                return instance;
            }

            return null;
        }
    }
}