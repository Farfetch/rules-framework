namespace Rules.Framework.Generics
{
    using System;

    /// <summary>
    /// Defines a generic rule
    /// </summary>
    public sealed class GenericRule
    {
        /// <summary>
        /// Gets and sets the if the rules ia active.
        /// </summary>
        public bool Active { get; internal set; }

        /// <summary>
        /// Gets the content which contains the rule content.
        /// </summary>
        public object Content { get; internal set; }

        /// <summary>
        /// Gets the content type.
        /// </summary>
        public GenericContentType ContentType { get; internal set; }

        /// <summary>
        /// Gets the date from which the rule begins being applicable.
        /// </summary>
        public DateTime DateBegin { get; internal set; }

        /// <summary>
        /// Gets and sets the date from which the rule ceases to be applicable.
        /// </summary>
        public DateTime? DateEnd { get; internal set; }

        /// <summary>
        /// Gets the rule name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the rule priority compared to other rules (preferrably it is unique).
        /// </summary>
        public int Priority { get; internal set; }

        /// <summary>
        /// Gets the rule root condition. This property is null when rule has no conditions.
        /// </summary>
        public GenericConditionNode RootCondition { get; internal set; }

        public static GenericRule Create(
            string name,
            GenericContentType contentType,
            object content,
            bool active,
            DateTime dateBegin,
            DateTime? dateEnd,
            int priority,
            GenericConditionNode rootCondition) => new GenericRule
            {
                Active = active,
                Content = content,
                ContentType = contentType,
                DateBegin = dateBegin,
                DateEnd = dateEnd,
                Name = name,
                Priority = priority,
                RootCondition = rootCondition,
            };
    }
}