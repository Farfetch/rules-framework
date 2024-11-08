namespace Rules.Framework.WebUI.ViewModels
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal sealed class RulesetViewModel
    {
        public int ActiveRulesCount { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Number { get; set; }

        public int TotalRulesCount { get; set; }
    }
}