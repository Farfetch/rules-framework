namespace Rules.Framework.Generic
{
    using System;
    using Rules.Framework.Core;

    public class GenericRule
    {
        /// <summary>
        /// Gets the content container which contains the rule content.
        /// </summary>
        public ContentContainer<ContentType> ContentContainer { get; internal set; }

        /// <summary>
        /// Gets the date from which the rule begins being applicable.
        /// </summary>
        public DateTime DateBegin { get; internal set; }

        /// <summary>
        /// Gets the date from which the rule ceases to be applicable.
        /// </summary>
        public DateTime? DateEnd { get; internal set; }

        /// <summary>
        /// Gets the rule name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the rule priority compared to other rules (preferrably it is unique).
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets the rule root condition. This property is null when rule has no conditions.
        /// </summary>
        public IConditionNode<ConditionType> RootCondition { get; internal set; }

        /// <summary>
        /// Clones the rule into a different instance.
        /// </summary>
        /// <returns></returns>
        public virtual GenericRule Clone()
            => new GenericRule
            {
                ContentContainer = this.ContentContainer,
                DateBegin = this.DateBegin,
                DateEnd = this.DateEnd,
                Name = this.Name,
                Priority = this.Priority,
                RootCondition = this.RootCondition?.Clone()
            };
    }
}