namespace Rules.Framework.Builder
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains the common results information from a rule build operation.
    /// </summary>
    public class RuleBuilderResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleBuilderResultBase"/> class.
        /// </summary>
        /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
        /// <param name="errors">The errors.</param>
        protected RuleBuilderResultBase(bool isSuccess, IEnumerable<string> errors)
        {
            this.IsSuccess = isSuccess;
            this.Errors = errors;
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>The errors.</value>
        public IEnumerable<string> Errors { get; }

        /// <summary>
        /// Gets a value indicating whether rule was built successfuly without validation errors.
        /// </summary>
        /// <value><c>true</c> if rule was built; otherwise, <c>false</c>.</value>
        public bool IsSuccess { get; }
    }
}