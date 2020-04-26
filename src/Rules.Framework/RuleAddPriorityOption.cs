namespace Rules.Framework
{
    /// <summary>
    /// The set of options to influence a new rule's priority when adding to data source.
    /// </summary>
    public class RuleAddPriorityOption
    {
        /// <summary>
        /// Gets or sets the rule name to use when <code>PriorityOptions.AtRuleName</code> option is selected.
        /// </summary>
        /// <value>
        /// At rule name option value.
        /// </value>
        public string AtRuleNameOptionValue { get; set; }

        /// <summary>
        /// Gets or sets the priority option.
        /// </summary>
        /// <value>
        /// The priority option.
        /// </value>
        public PriorityOptions PriorityOption { get; set; }
    }
}