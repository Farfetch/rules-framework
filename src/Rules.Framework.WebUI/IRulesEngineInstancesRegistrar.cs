namespace Rules.Framework.WebUI
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the registration of <see cref="IRulesEngine"/> instances on the Web UI.
    /// </summary>
    public interface IRulesEngineInstancesRegistrar
    {
        /// <summary>
        /// Adds a rules engine instance to be presented and used on Web UI.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="getRulesEngineFunc">The get rules engine function.</param>
        /// <returns></returns>
        IRulesEngineInstancesRegistrar AddInstance(string name, Func<IServiceProvider, string, IRulesEngine> getRulesEngineFunc);

        /// <summary>
        /// Adds a rules engine instance to be presented and used on Web UI.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="getRulesEngineFunc">The get rules engine function.</param>
        /// <returns></returns>
        IRulesEngineInstancesRegistrar AddInstance(string name, Func<IServiceProvider, string, Task<IRulesEngine>> getRulesEngineFunc);
    }
}