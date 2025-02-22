namespace Rules.Framework.WebUI.ViewModels
{
    /// <summary>
    /// The view model for condition nodes.
    /// </summary>
    public class ConditionNodeViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionNodeViewModel"/> class.
        /// </summary>
        internal ConditionNodeViewModel()
        {
        }

        /// <summary>
        /// Gets the logical operator.
        /// </summary>
        /// <value>The logical operator.</value>
        public string LogicalOperator { get; internal set; }
    }
}