namespace Rules.Framework.Rql
{
    /// <summary>
    /// Defines the common result of a Rule Query Language source evaluation.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Gets the Rule Query Language source.
        /// </summary>
        /// <value>The Rule Query Language source.</value>
        string Rql { get; }
    }
}