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
        /// Gets or sets the content type to filter.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the name to filter.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the priority to filter.
        /// </summary>
        /// <value>The priority.</value>
        public int? Priority { get; set; }
    }
}