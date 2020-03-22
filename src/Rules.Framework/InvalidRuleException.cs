using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rules.Framework
{
    /// <summary>
    /// Exception thrown when a operation is attempted with a invalid rule.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class InvalidRuleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRuleException"/> class.
        /// </summary>
        public InvalidRuleException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRuleException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidRuleException(string message)
            : base(message)
        {
            this.Errors = Enumerable.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRuleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors.</param>
        public InvalidRuleException(
            string message,
            IEnumerable<string> errors)
            : base(message)
        {
            this.Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRuleException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public InvalidRuleException(
            string message,
            Exception innerException)
            : base(message, innerException)
        {
            this.Errors = Enumerable.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRuleException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidRuleException(
            string message,
            IEnumerable<string> errors,
            Exception innerException)
            : base(message, innerException)
        {
            this.Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRuleException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected InvalidRuleException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            this.Errors = info.GetValue(nameof(this.Errors), typeof(IEnumerable<string>)) as IEnumerable<string>;
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        public IEnumerable<string> Errors { get; }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="SerializationInfo"></see> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"></see> that contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(this.Errors), this.Errors);
        }
    }
}