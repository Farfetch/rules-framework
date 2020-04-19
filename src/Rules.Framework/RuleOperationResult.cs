namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RuleOperationResult
    {
        protected RuleOperationResult(bool isSuccess, IEnumerable<string> errors)
        {
            this.IsSuccess = isSuccess;
            this.Errors = errors;
        }

        public IEnumerable<string> Errors { get; }
        public bool IsSuccess { get; }

        internal static RuleOperationResult Error(IEnumerable<string> errors)
        {
            if (errors is { })
            {
                throw new ArgumentNullException(nameof(errors));
            }

            return new RuleOperationResult(false, errors);
        }

        internal static RuleOperationResult Success() => new RuleOperationResult(true, Enumerable.Empty<string>());
    }
}