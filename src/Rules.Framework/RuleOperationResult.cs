namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents the result of a rule operation.
    /// </summary>
    public class RuleOperationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleOperationResult"/> class.
        /// </summary>
        /// <param name="isSuccess">if set to <c>true</c> [is success].</param>
        /// <param name="errors">The errors.</param>
        protected RuleOperationResult(bool isSuccess, IEnumerable<string> errors)
        {
            this.IsSuccess = isSuccess;
            this.Errors = errors;
        }

        /// <summary>
        /// Gets the errors occurred during rule operation.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        public IEnumerable<string> Errors { get; }

        /// <summary>
        /// Gets a value indicating whether rule operation was successfull or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if rule operation was successfull; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess { get; }

        internal static RuleOperationResult Error(IEnumerable<string> errors)
        {
            if (errors is null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            return new RuleOperationResult(false, errors);
        }

        internal static RuleOperationResult Success() => new RuleOperationResult(true, Enumerable.Empty<string>());
    }
}