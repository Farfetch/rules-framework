namespace Rules.Framework
{
    using System;
    using Rules.Framework.Builder;
    using Rules.Framework.Builder.Generic;

    /// <summary>
    /// Defines a rule.
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// Gets and sets the if the rules is active.
        /// </summary>
        public bool Active { get; internal set; } = true;

        /// <summary>
        /// Gets the content container which contains the rule content.
        /// </summary>
        public ContentContainer ContentContainer { get; internal set; }

        /// <summary>
        /// Gets the date from which the rule begins being applicable.
        /// </summary>
        public DateTime DateBegin { get; internal set; }

        /// <summary>
        /// Gets and sets the date from which the rule ceases to be applicable.
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Gets the rule name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets and sets the rule priority compared to other rules (preferably it is unique).
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets the rule root condition. This property is null when rule has no conditions.
        /// </summary>
        public IConditionNode RootCondition { get; internal set; }

        /// <summary>
        /// Creates a new rule with generic content type and condition type.
        /// </summary>
        /// <typeparam name="TContentType">The type of the content type.</typeparam>
        /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
        /// <returns></returns>
        public static IRuleBuilder<TContentType, TConditionType> New<TContentType, TConditionType>()
            => new RuleBuilder<TContentType, TConditionType>();

        /// <summary>
        /// Creates a new rule.
        /// </summary>
        /// <returns></returns>
        public static IRuleBuilder New()
            => new RuleBuilder();

        /// <summary>
        /// Clones the rule into a different instance.
        /// </summary>
        /// <returns></returns>
        public virtual Rule Clone()
            => new Rule
            {
                ContentContainer = this.ContentContainer,
                DateBegin = this.DateBegin,
                DateEnd = this.DateEnd,
                Name = this.Name,
                Priority = this.Priority,
                RootCondition = this.RootCondition?.Clone(),
                Active = this.Active,
            };
    }
}