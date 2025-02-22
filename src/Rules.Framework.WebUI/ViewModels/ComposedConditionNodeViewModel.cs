namespace Rules.Framework.WebUI.ViewModels
{
    using System.Collections.Generic;

    /// <summary>
    /// The view model for composed condition nodes.
    /// </summary>
    /// <seealso cref="Rules.Framework.WebUI.ViewModels.ConditionNodeViewModel"/>
    public sealed class ComposedConditionNodeViewModel : ConditionNodeViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComposedConditionNodeViewModel"/> class.
        /// </summary>
        internal ComposedConditionNodeViewModel()
        {
        }

        /// <summary>
        /// Gets the child condition nodes.
        /// </summary>
        /// <value>The child condition nodes.</value>
        public IEnumerable<ConditionNodeViewModel> ChildConditionNodes { get; internal set; }
    }
}