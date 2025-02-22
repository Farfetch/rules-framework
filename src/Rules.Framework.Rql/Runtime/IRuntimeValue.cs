namespace Rules.Framework.Rql.Runtime
{
    using System;
    using Rules.Framework.Rql.Runtime.Types;

    /// <summary>
    /// Defines the interface contract for representing a RQL runtime type under the .NET runtime.
    /// </summary>
    internal interface IRuntimeValue
    {
        /// <summary>
        /// Gets the .NET runtime type.
        /// </summary>
        /// <value>The .NET runtime type.</value>
        Type RuntimeType { get; }

        /// <summary>
        /// Gets the .NET runtime value.
        /// </summary>
        /// <value>The .NET runtime value.</value>
        object RuntimeValue { get; }

        /// <summary>
        /// Gets the RQL runtime type.
        /// </summary>
        /// <value>The RQL runtime type.</value>
        RqlType Type { get; }
    }
}