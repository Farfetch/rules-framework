namespace Rules.Framework
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The set of arguments to filter rules.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RulesFilterArgs
    {
        /// <summary>
        /// Gets or sets the name to filter.
        /// </summary>
        /// <value>The name.</value>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the priority to filter.
        /// </summary>
        /// <value>The priority.</value>
        public int? Priority { get; set; }

        /// <summary>
        /// Gets or sets the ruleset to filter.
        /// </summary>
        /// <value>The ruleset name.</value>
        public string? Ruleset { get; set; }
    }
}