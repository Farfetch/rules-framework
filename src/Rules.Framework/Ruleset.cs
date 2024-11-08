namespace Rules.Framework
{
    using System;

    /// <summary>
    /// The representation of a ruleset and associated metadata.
    /// </summary>
    public class Ruleset
    {
        internal Ruleset(string name, DateTime creation)
        {
            this.Name = name;
            this.Creation = creation;
        }

        /// <summary>
        /// Gets the creation date and time of the ruleset.
        /// </summary>
        /// <value>The creation.</value>
        public DateTime Creation { get; }

        /// <summary>
        /// Gets the name of the ruleset.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }
    }
}