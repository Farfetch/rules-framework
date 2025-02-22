namespace Rules.Framework.WebUI.ViewModels
{
    /// <summary>
    /// The view model for value condition nodes.
    /// </summary>
    /// <seealso cref="Rules.Framework.WebUI.ViewModels.ConditionNodeViewModel"/>
    public sealed class ValueConditionNodeViewModel : ConditionNodeViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConditionNodeViewModel"/> class.
        /// </summary>
        internal ValueConditionNodeViewModel()
        {
        }

        /// <summary>
        /// Gets the condition.
        /// </summary>
        /// <value>The condition.</value>
        public string Condition { get; internal set; }

        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        public string DataType { get; internal set; }

        /// <summary>
        /// Gets the operand.
        /// </summary>
        /// <value>The operand.</value>
        public dynamic Operand { get; internal set; }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <value>The operator.</value>
        public string Operator { get; internal set; }
    }
}