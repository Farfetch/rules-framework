namespace Rules.Framework.WebUI.ViewModels
{
    using System.Collections.Generic;

    internal sealed class ComposedConditionNodeViewModel : ConditionNodeViewModel
    {
        public IEnumerable<ConditionNodeViewModel> ChildConditionNodes { get; internal set; }
    }
}