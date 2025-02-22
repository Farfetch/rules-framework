namespace Rules.Framework.Rql
{
    /// <summary>
    /// Defines a assist suggestion for a Rule Query Language source on a particular evaluated position.
    /// </summary>
    public interface IAssistSuggestion
    {
        /// <summary>
        /// Gets the lexeme suggested for the evaluated position.
        /// </summary>
        /// <value>The lexeme.</value>
        string Lexeme { get; }
    }
}