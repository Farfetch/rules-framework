namespace Rules.Framework.WebUI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal sealed class RulesEngineInstanceViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<OptionViewModel> Options { get; set; }
    }
}