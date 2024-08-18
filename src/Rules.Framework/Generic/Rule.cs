namespace Rules.Framework.Generic
{
    using System;

    /// <summary>
    /// Defines a rule.
    /// </summary>
    /// <typeparam name="TContentType">The content type that allows to categorize rules.</typeparam>
    /// <typeparam name="TConditionType">
    /// The condition type that allows to filter rules based on a set of conditions.
    /// </typeparam>
    public class Rule<TContentType, TConditionType>
    {
        private static readonly Type stringType = typeof(string);

        private readonly Rule wrappedRule;
        private IConditionNode<TConditionType>? rootCondition;

        internal Rule(Rule wrappedRule)
        {
            var typeForContentType = typeof(TContentType);
            if (!typeForContentType.IsEnum && typeForContentType != stringType)
            {
                throw new NotSupportedException($"Only enum types or string are supported as {nameof(TContentType)}.");
            }

            var typeForConditionType = typeof(TConditionType);
            if (!typeForConditionType.IsEnum && typeForConditionType != stringType)
            {
                throw new NotSupportedException($"Only enum types or string are supported as {nameof(TConditionType)}.");
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
        /// Gets the content type.
        /// </summary>
        public TContentType ContentType => GenericConversions.Convert<TContentType>(this.wrappedRule.ContentType);

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
        public IConditionNode<TConditionType> RootCondition
        {
            get
            {
                this.rootCondition ??= this.wrappedRule.RootCondition?.ToGenericConditionNode<TConditionType>();
                return this.rootCondition!;
            }
        }

        /// <summary>
        /// Obtains the non-generic <see cref="Rule"/> contained on the given instance of <see
        /// cref="Rule{TContentType, TConditionType}"/>.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns>The non-generic rule.</returns>
        public static implicit operator Rule(Rule<TContentType, TConditionType> rule) => rule.wrappedRule;

        /// <summary>
        /// Clones the rule into a different instance.
        /// </summary>
        /// <returns></returns>
        public virtual Rule<TContentType, TConditionType> Clone() => new(this.wrappedRule.Clone());
    }
}