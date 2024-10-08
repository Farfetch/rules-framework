namespace Rules.Framework.Builder.RulesBuilder
{
    using System;

    /// <summary>
    /// Configurer for a rule's begin date.
    /// </summary>
    public interface IRuleConfigureDateBegin
    {
        /// <summary>
        /// Sets the new rule with the specified date begin.
        /// </summary>
        /// <param name="dateBegin">The date begin.</param>
        /// <returns></returns>
        IRuleConfigureDateEndOptional Since(DateTime dateBegin);
    }
}