namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The set of search arguments to find rules.
    /// </summary>
    /// <typeparam name="TRuleset">The type of the ruleset.</typeparam>
    /// <typeparam name="TCondition">The type of the condition.</typeparam>
    public class SearchArgs<TRuleset, TCondition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchArgs{TRuleset, TCondition}"/> class.
        /// </summary>
        /// <param name="ruleset">The ruleset name.</param>
        /// <param name="dateBegin">The date begin.</param>
        /// <param name="dateEnd">The date end.</param>
        public SearchArgs(TRuleset ruleset, DateTime dateBegin, DateTime dateEnd)
        {
            this.Conditions = new Dictionary<TCondition, object>();
            this.Ruleset = ruleset;
            this.DateBegin = dateBegin;
            this.DateEnd = dateEnd;
            this.ExcludeRulesWithoutSearchConditions = false;
            this.Active = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchArgs{TRuleset, TCondition}"/> class.
        /// </summary>
        /// <param name="ruleset">The ruleset name.</param>
        /// <param name="dateBegin">The date begin.</param>
        /// <param name="dateEnd">The date end.</param>
        /// <param name="active">The active status.</param>
        public SearchArgs(TRuleset ruleset, DateTime dateBegin, DateTime dateEnd, bool active)
        {
            this.Conditions = new Dictionary<TCondition, object>();
            this.Ruleset = ruleset;
            this.DateBegin = dateBegin;
            this.DateEnd = dateEnd;
            this.ExcludeRulesWithoutSearchConditions = false;
            this.Active = active;
        }

        /// <summary>
        /// Gets or sets the active status.
        /// </summary>
        /// <value>The active status.</value>
        public bool? Active { get; }

        /// <summary>
        /// Gets or sets the search conditions.
        /// </summary>
        /// <value>The conditions.</value>
        public IDictionary<TCondition, object> Conditions { get; set; }

        /// <summary>
        /// Gets or sets the date begin.
        /// </summary>
        /// <value>The date begin.</value>
        public DateTime DateBegin { get; }

        /// <summary>
        /// Gets or sets the date end.
        /// </summary>
        /// <value>The date end.</value>
        public DateTime DateEnd { get; }

        /// <summary>
        /// Gets or sets whether rules without search conditions should or not be excluded.
        /// </summary>
        /// <value><c>true</c> if [exclude rules without search conditions]; otherwise, <c>false</c>.</value>
        public bool ExcludeRulesWithoutSearchConditions { get; set; }

        /// <summary>
        /// Gets or sets the ruleset to search.
        /// </summary>
        /// <value>The ruleset name.</value>
        public TRuleset Ruleset { get; }
    }
}