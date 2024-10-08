namespace Rules.Framework.Builder.Generic.RulesBuilder
{
    using System;

    /// <summary>
    /// Configurer for a rule's end date.
    /// </summary>
    /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
    /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
    public interface IRuleConfigureDateEnd<TRuleset, TCondition>
    {
        /// <summary>
        /// Sets the new rule with the specified date end.
        /// </summary>
        /// <param name="dateEnd">The date end.</param>
        /// <returns></returns>
        IRuleBuilder<TRuleset, TCondition> Until(DateTime? dateEnd);
    }
}