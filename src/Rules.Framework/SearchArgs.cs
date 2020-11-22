namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The set of search arguments to find rules.
    /// </summary>
    /// <typeparam name="TContentType">The type of the content type.</typeparam>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    public class SearchArgs<TContentType, TConditionType>
    {
        /// <summary>
        /// Gets or sets the search conditions.
        /// </summary>
        /// <value>
        /// The conditions.
        /// </value>
        public IEnumerable<Condition<TConditionType>> Conditions { get; set; }

        /// <summary>
        /// Gets or sets the content type to search.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public TContentType ContentType { get; set; }

        /// <summary>
        /// Gets or sets the date begin.
        /// </summary>
        /// <value>
        /// The date begin.
        /// </value>
        public DateTime DateBegin { get; set; }

        /// <summary>
        /// Gets or sets the date end.
        /// </summary>
        /// <value>
        /// The date end.
        /// </value>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Gets or sets whether rules without search conditions should or not be excluded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [exclude rules without search conditions]; otherwise, <c>false</c>.
        /// </value>
        public bool ExcludeRulesWithoutSearchConditions { get; set; }
    }
}