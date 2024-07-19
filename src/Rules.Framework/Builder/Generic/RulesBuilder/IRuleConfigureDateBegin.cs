namespace Rules.Framework.Builder.Generic.RulesBuilder
{
    using System;

    /// <summary>
    /// Configurer for a rule's begin date.
    /// </summary>
    /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
    /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
    public interface IRuleConfigureDateBegin<TRuleset, TCondition>
    {
        /// <summary>
        /// Sets the new rule with the specified date begin.
        /// </summary>
        /// <param name="dateBegin">The date begin.</param>
        /// <returns></returns>
        IRuleConfigureDateEndOptional<TRuleset, TCondition> Since(DateTime dateBegin);
    }
}