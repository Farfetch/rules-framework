namespace Rules.Framework.Core
{
    /// <summary>
    /// Defines the set of operators supported for rule's conditions.
    /// </summary>
    public enum Operators
    {
        /// <summary>
        /// The equal operator.
        /// </summary>
        Equal = 1,

        /// <summary>
        /// The not equal operator.
        /// </summary>
        NotEqual = 2,

        /// <summary>
        /// The greater than operator.
        /// </summary>
        GreaterThan = 3,

        /// <summary>
        /// The greater than or equal operator.
        /// </summary>
        GreaterThanOrEqual = 4,

        /// <summary>
        /// The lesser than operator.
        /// </summary>
        LesserThan = 5,

        /// <summary>
        /// The lesser than or equal operator.
        /// </summary>
        LesserThanOrEqual = 6,

        /// <summary>
        /// The contains operator.
        /// </summary>
        Contains = 7
    }
}