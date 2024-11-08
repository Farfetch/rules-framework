namespace Rules.Framework.Builder.RulesBuilder
{
    using System;

    /// <summary>
    /// Configurer for a rule's end date.
    /// </summary>
    public interface IRuleConfigureDateEnd
    {
        /// <summary>
        /// Sets the new rule with the specified date end.
        /// </summary>
        /// <param name="dateEnd">The date end.</param>
        /// <returns></returns>
        IRuleBuilder Until(DateTime? dateEnd);
    }
}