namespace Rules.Framework.WebUI
{
    using System;
    using System.Threading.Tasks;

    public interface IRulesEngineInstancesRegistrar
    {
        IRulesEngineInstancesRegistrar AddInstance(string name, Func<IServiceProvider, string, IRulesEngine> getRulesEngineFunc);

        IRulesEngineInstancesRegistrar AddInstance(string name, Func<IServiceProvider, string, Task<IRulesEngine>> getRulesEngineFunc);
    }
}