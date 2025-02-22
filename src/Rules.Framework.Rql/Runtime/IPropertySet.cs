namespace Rules.Framework.Rql.Runtime
{
    using Rules.Framework.Rql.Runtime.Types;

    /// <summary>
    /// Defines the interface contract for representing a RQL runtime type that exposes settable properties.
    /// </summary>
    internal interface IPropertySet
    {
        /// <summary>
        /// Sets the property value of a RQL value.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <returns></returns>
        RqlAny SetPropertyValue(RqlString name, RqlAny value);
    }
}