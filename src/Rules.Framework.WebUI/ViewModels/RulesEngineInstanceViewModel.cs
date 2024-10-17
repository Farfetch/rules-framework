namespace Rules.Framework.WebUI.ViewModels
{
    using System;
    using System.Collections.Generic;

    internal class RulesEngineInstanceViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IDictionary<string, object> Options { get; set; }
    }
}