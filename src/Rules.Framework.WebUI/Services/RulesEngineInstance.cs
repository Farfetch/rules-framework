namespace Rules.Framework.WebUI.Services
{
    using System;

    internal class RulesEngineInstance
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IRulesEngine RulesEngine { get; set; }
    }
}