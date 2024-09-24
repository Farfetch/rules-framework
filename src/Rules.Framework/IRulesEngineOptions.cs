namespace Rules.Framework
{
    using System.Collections.Generic;

    /// <summary>
    /// The set of rules engine options that influence rules engine behavior.
    /// </summary>
    public interface IRulesEngineOptions
    {
        /// <summary>
        /// Gets a value indicating whether automatic creation of content types is enabled, allowing
        /// them to be added when a rule is added, when enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if content types should be automatically created on rule add; otherwise, <c>false</c>.
        /// </value>
        bool AutoCreateContentTypes { get; }

        /// <summary>
        /// Gets the default values for each of the supported data types.
        /// </summary>
        public IDictionary<DataTypes, object> DataTypeDefaults { get; }

        /// <summary>
        /// Gets the rules engine behavior when no condition with a specific type is provided to
        /// rules engine to match with a rule's condition with the same type.
        /// </summary>
        public MissingConditionBehaviors MissingConditionBehavior { get; }

        /// <summary>
        /// Gets the priority criteria to untie when multiples rules are matched.
        /// </summary>
        public PriorityCriterias PriorityCriteria { get; }
    }
}