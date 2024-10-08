namespace Rules.Framework.Generic
{
    using System;
    using Rules.Framework.Core;

    /// <summary>
    /// Defines a rule.
    /// </summary>
    /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
    /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
    public class Rule<TRuleset, TCondition>
    {
        private readonly Rule wrappedRule;
        private IConditionNode<TCondition>? rootCondition;

        internal Rule(Rule wrappedRule)
        {
            var typeForRuleset = typeof(TRuleset);
            if (!typeForRuleset.IsEnum && typeForRuleset != TypesCache.String)
            {
                throw new NotSupportedException($"Only enum types or string are supported as {nameof(TRuleset)}.");
            }

            var typeForCondition = typeof(TCondition);
            if (!typeForCondition.IsEnum && typeForCondition != TypesCache.String)
            {
                throw new NotSupportedException($"Only enum types or string are supported as {nameof(TCondition)}.");
            }

            this.wrappedRule = wrappedRule ?? throw new ArgumentNullException(nameof(wrappedRule));
        }

        /// <summary>
        /// Gets if the rule is active.
        /// </summary>
        public bool Active => this.wrappedRule.Active;

        /// <summary>
        /// Gets the content container which contains the rule content.
        /// </summary>
        public ContentContainer ContentContainer => this.wrappedRule.ContentContainer;

        /// <summary>
        /// Gets the date from which the rule begins being applicable.
        /// </summary>
        public DateTime DateBegin => this.wrappedRule.DateBegin;

        /// <summary>
        /// Gets and sets the date from which the rule ceases to be applicable.
        /// </summary>
        public DateTime? DateEnd
        {
            get => this.wrappedRule.DateEnd;
            set => this.wrappedRule.DateEnd = value;
        }

        /// <summary>
        /// Gets the rule name.
        /// </summary>
        public string Name => this.wrappedRule.Name;

        /// <summary>
        /// Gets and sets the rule priority compared to other rules (preferably it is unique).
        /// </summary>
        public int Priority
        {
            get => this.wrappedRule.Priority;
            set => this.wrappedRule.Priority = value;
        }

        /// <summary>
        /// Gets the rule root condition. This property is null when rule has no conditions.
        /// </summary>
        public IConditionNode<TCondition> RootCondition
        {
            get
            {
                this.rootCondition ??= this.wrappedRule.RootCondition?.ToGenericConditionNode<TCondition>();
                return this.rootCondition!;
            }
        }

        /// <summary>
        /// Gets the ruleset to which the rule belongs to.
        /// </summary>
        public TRuleset Ruleset => GenericConversions.Convert<TRuleset>(this.wrappedRule.Ruleset);

        /// <summary>
        /// Obtains the non-generic <see cref="Rule"/> contained on the given instance of <see
        /// cref="Rule{TRuleset, TCondition}"/>.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>The non-generic rule.</returns>
        public static implicit operator Rule(Rule<TRuleset, TCondition> rule) => rule.wrappedRule;

        /// <summary>
        /// Clones the rule into a different instance.
        /// </summary>
        /// <returns></returns>
        public virtual Rule<TRuleset, TCondition> Clone() => new(this.wrappedRule.Clone());
    }
}