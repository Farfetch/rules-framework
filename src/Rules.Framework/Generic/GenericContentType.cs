namespace Rules.Framework.Generic
{
    using System;

    /// <summary>
    /// Defines generic content type
    /// </summary>
    public struct GenericContentType : IEquatable<GenericContentType>
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        /// <value>The file name.</value>
        public string FileName { get; set; }

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
        /// true if the current object is equal to the <paramref name="other">other</paramref>
        /// parameter; otherwise, false.
        /// </returns>
        public bool Equals(GenericContentType other)
        {
            return other.FileName == this.FileName
                && other.Name == this.Name;
        }
    }
}