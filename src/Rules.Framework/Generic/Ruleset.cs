namespace Rules.Framework.Generic
{
    using System;

    /// <summary>
    /// Defines a ruleset that groups inter-related rules.
    /// </summary>
    /// <typeparam name="TRuleset">The type of the ruleset.</typeparam>
    public class Ruleset<TRuleset>
    {
        private readonly Ruleset wrappedRuleset;

        internal Ruleset(Ruleset wrappedRuleset)
        {
            this.wrappedRuleset = wrappedRuleset;
        }

        /// <summary>
        /// Gets the creation date and time of the ruleset.
        /// </summary>
        /// <value>The creation.</value>
        public DateTime Creation => this.wrappedRuleset.Creation;

        /// <summary>
        /// Gets the name of the ruleset.
        /// </summary>
        /// <value>The name.</value>
        public TRuleset Name => GenericConversions.Convert<TRuleset>(this.wrappedRuleset.Name);
    }
}