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
        Boolean = 4,

        /// <summary>
        /// The array integer data type for condition nodes based on array integer values.
        /// </summary>
        ArrayInteger = 5,

        /// <summary>
        /// The array decimal data type for condition nodes based on array decimal values.
        /// </summary>
        ArrayDecimal = 6,

        /// <summary>
        /// The array string data type for condition nodes based on array string values.
        /// </summary>
        ArrayString = 7,

        /// <summary>
        /// The array boolean data type for condition nodes based on array boolean values.
        /// </summary>
        ArrayBoolean = 8
    }
}