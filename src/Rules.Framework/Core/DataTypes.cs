namespace Rules.Framework.Core
{
    /// <summary>
    /// Defines the supported data types a condition node can assume.
    /// </summary>
    public enum DataTypes
    {
        /// <summary>
        /// The integer data type for condition nodes based on integer values.
        /// </summary>
        Integer = 1,

        /// <summary>
        /// The decimal data type for condition nodes based on decimal values.
        /// </summary>
        Decimal = 2,

        /// <summary>
        /// The string data type for condition nodes based on string values.
        /// </summary>
        String = 3,

        /// <summary>
        /// The boolean data type for condition nodes based on boolean values.
        /// </summary>
        Boolean = 4
    }
}