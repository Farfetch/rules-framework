namespace Rules.Framework.WebUI.ViewModels
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal sealed class OptionViewModel
    {
        public string Name { get; set; }

        public string NameDescription { get; set; }

        public object Value { get; set; }

        public string ValueDescription { get; set; }
    }
}