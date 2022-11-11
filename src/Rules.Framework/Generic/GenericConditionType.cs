namespace Rules.Framework.Generic
{
    using System;

    /// <summary>
    /// Defines generic condition type
    /// </summary>
    public struct GenericConditionType : IEquatable<GenericConditionType>
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(GenericConditionType other)
        {
            return other.Name == this.Name
                && other.Code == this.Code;
        }
    }
}