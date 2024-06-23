namespace Rules.Framework.Generics
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Exposes generic rules engine logic to provide rule matches to requests.
    /// </summary>
    public interface IGenericRulesEngine : IRulesEngine<string, string>
    {
        /// <summary>
        /// Gets the content types.
        /// </summary>
        /// <returns>List of content types</returns>
        /// <exception cref="System.ArgumentException">
        /// Method only works if TContentType is a enum
        /// </exception>
        IEnumerable<string> GetContentTypes();
    }
}