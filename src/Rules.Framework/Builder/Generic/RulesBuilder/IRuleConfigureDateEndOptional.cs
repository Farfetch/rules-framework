namespace Rules.Framework.Builder.Generic.RulesBuilder
{
    /// <summary>
    /// Configurer for a rule's optional end date or proceed to remaining rule build options.
    /// </summary>
    /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
    /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
    public interface IRuleConfigureDateEndOptional<TRuleset, TCondition> :
        IRuleConfigureDateEnd<TRuleset, TCondition>,
        IRuleBuilder<TRuleset, TCondition>
    {
    }
}