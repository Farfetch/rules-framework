namespace Rules.Framework
{
    /// <summary>
    /// Defines the logical operators to use between multiple rule's condition nodes.
    /// </summary>
    public enum LogicalOperators
    {
        /// <summary>
        /// Applies a AND between a set of condition nodes.
        /// </summary>
        And = 1,

        /// <summary>
        /// Applies a OR between a set of condition nodes.
        /// </summary>
        Or = 2,

        /// <summary>
        /// Evals a condition node (only applicable when not in a composed condition node).
        /// </summary>
        Eval = 3
    }
}