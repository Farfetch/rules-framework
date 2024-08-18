namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents the result of an operation performed on the rules engine.
    /// </summary>
    public class OperationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        /// <param name="isSuccess">if set to <c>true</c> it represents a success operation result.</param>
        /// <param name="errors">The errors.</param>
        protected OperationResult(bool isSuccess, IEnumerable<string> errors)
        {
            this.IsSuccess = isSuccess;
            this.Errors = errors;
        }

        /// <summary>
        /// Gets the errors occurred during the operation.
        /// </summary>
        /// <value>The errors.</value>
        public IEnumerable<string> Errors { get; }

        /// <summary>
        /// Gets a value indicating whether the operation was successfull or not.
        /// </summary>
        /// <value><c>true</c> if rule operation was successfull; otherwise, <c>false</c>.</value>
        public bool IsSuccess { get; }

        internal static OperationResult Error(string error) => Error(new[] { error });

        internal static OperationResult Error(IEnumerable<string> errors)
        {
            if (errors is null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            return new OperationResult(false, errors);
        }

        internal static OperationResult Success() => new OperationResult(true, Enumerable.Empty<string>());
    }
}