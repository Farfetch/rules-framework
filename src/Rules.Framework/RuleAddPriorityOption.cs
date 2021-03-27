namespace Rules.Framework
{
    /// <summary>
    /// The set of options to influence a new rule's priority when adding to data source.
    /// </summary>
    public class RuleAddPriorityOption
    {
        /// <summary>
        /// Creates a <see cref="RuleAddPriorityOption"/> setted to placed at bottom of priority.
        /// </summary>
        public static RuleAddPriorityOption AtBottom => new RuleAddPriorityOption
        {
            AtRuleNameOptionValue = null,
            PriorityOption = PriorityOptions.AtBottom
        };

        /// <summary>
        /// Creates a <see cref="RuleAddPriorityOption"/> setted to placed at top of priority.
        /// </summary>
        public static RuleAddPriorityOption AtTop => new RuleAddPriorityOption
        {
            AtRuleNameOptionValue = null,
            PriorityOption = PriorityOptions.AtTop
        };

        /// <summary>
        /// Gets or sets the rule name to use when
        /// <code>PriorityOptions.AtRuleName</code>
        /// option is selected.
        /// </summary>
        /// <value>At rule name option value.</value>
        public string AtRuleNameOptionValue { get; set; }

        /// <summary>
        /// Gets or sets the priority option.
        /// </summary>
        /// <value>The priority option.</value>
        public PriorityOptions PriorityOption { get; set; }

        /// <summary>
        /// Creates a <see cref="RuleAddPriorityOption"/> setted by rule name.
        /// </summary>
        /// <param name="ruleName">Name of the rule.</param>
        /// <returns></returns>
        public static RuleAddPriorityOption ByRuleName(string ruleName) => new RuleAddPriorityOption
        {
            AtRuleNameOptionValue = ruleName,
            PriorityOption = PriorityOptions.AtRuleName
        };
    }
}