namespace Rules.Framework.WebUI.ViewModels
{
    using System.Collections.Generic;

    public sealed class ComposedConditionNodeViewModel : ConditionNodeViewModel
    {
        internal ComposedConditionNodeViewModel()
        {
        }

        public IEnumerable<ConditionNodeViewModel> ChildConditionNodes { get; internal set; }
    }
}