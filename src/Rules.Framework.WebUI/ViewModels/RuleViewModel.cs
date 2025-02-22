namespace Rules.Framework.WebUI.ViewModels
{
    using System;

    /// <summary>
    /// The view model for rules.
    /// </summary>
    public sealed class RuleViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleViewModel"/> class.
        /// </summary>
        internal RuleViewModel()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RuleViewModel"/> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the conditions.
        /// </summary>
        /// <value>The conditions.</value>
        public string Conditions { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content { get; set; }

        /// <summary>
        /// Gets or sets the date begin.
        /// </summary>
        /// <value>The date begin.</value>
        public DateTime DateBegin { get; set; }

        /// <summary>
        /// Gets or sets the date end.
        /// </summary>
        /// <value>The date end.</value>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the root condition.
        /// </summary>
        /// <value>The root condition.</value>
        public ConditionNodeViewModel RootCondition { get; set; }

        /// <summary>
        /// Gets or sets the ruleset.
        /// </summary>
        /// <value>The ruleset.</value>
        public string Ruleset { get; set; }
    }
}