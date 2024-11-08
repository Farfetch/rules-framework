namespace Rules.Framework.WebUI.Services
{
    using System;
    using System.Collections.Generic;

    internal interface IRulesEngineInstanceProvider
    {
        IEnumerable<RulesEngineInstance> GetAllInstances();

        RulesEngineInstance GetInstance(Guid instanceId);
    }
}