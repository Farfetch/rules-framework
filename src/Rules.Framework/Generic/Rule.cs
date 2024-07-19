namespace Rules.Framework.Generic
{
    using System;
    using Rules.Framework.Core;

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
        private ContentContainer<TContentType>? contentContainer;
        private IConditionNode<TConditionType>? rootCondition;

        internal Rule()
            : this(new Rule())
        {
        }

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
        /// Gets and sets the if the rules is active.
        /// </summary>
        public bool Active
        {
            get => this.wrappedRule.Active;
            internal set => this.wrappedRule.Active = value;
        }

        /// <summary>
        /// Gets the content container which contains the rule content.
        /// </summary>
        public ContentContainer<TContentType> ContentContainer
        {
            get
            {
                this.contentContainer ??= new ContentContainer<TContentType>(
                    GenericConversions.Convert<TContentType>(this.wrappedRule.ContentContainer.ContentType),
                    this.wrappedRule.ContentContainer.ContentFunc);

                return this.contentContainer;
            }
            internal set
            {
                this.contentContainer = value;
                this.wrappedRule.ContentContainer = new ContentContainer(GenericConversions.Convert(value.ContentType), value.ContentFunc);
            }
        }

        /// <summary>
        /// Gets the date from which the rule begins being applicable.
        /// </summary>
        public DateTime DateBegin
        {
            get => this.wrappedRule.DateBegin;
            internal set => this.wrappedRule.DateBegin = value;
        }

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
        public string Name
        {
            get => this.wrappedRule.Name;
            internal set => this.wrappedRule.Name = value;
        }

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
                return this.rootCondition;
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