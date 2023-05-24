namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The set of search arguments to find rules.
    /// </summary>
    /// <typeparam name="TContentType">The type of the content type.</typeparam>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    public class SearchArgs<TContentType, TConditionType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchArgs{TContentType, TConditionType}"/> class.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="dateBegin">The date begin.</param>
        /// <param name="dateEnd">The date end.</param>
        public SearchArgs(TContentType contentType, DateTime dateBegin, DateTime dateEnd)
        {
            this.Conditions = Enumerable.Empty<Condition<TConditionType>>();
            this.ContentType = contentType;
            this.DateBegin = dateBegin;
            this.DateEnd = dateEnd;
            this.ExcludeRulesWithoutSearchConditions = false;
            this.Active = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchArgs{TContentType, TConditionType}"/> class.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="dateBegin">The date begin.</param>
        /// <param name="dateEnd">The date end.</param>
        /// <param name="active">The active status.</param>
        public SearchArgs(TContentType contentType, DateTime dateBegin, DateTime dateEnd, bool active)
        {
            this.Conditions = Enumerable.Empty<Condition<TConditionType>>();
            this.ContentType = contentType;
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
        public IEnumerable<Condition<TConditionType>> Conditions { get; set; }

        /// <summary>
        /// Gets or sets the content type to search.
        /// </summary>
        /// <value>The type of the content.</value>
        public TContentType ContentType { get; }

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
    }
}
