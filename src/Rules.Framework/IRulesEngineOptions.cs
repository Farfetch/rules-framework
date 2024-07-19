namespace Rules.Framework
{
    using System.Collections.Generic;

    /// <summary>
    /// The set of rules engine options that influence rules engine behavior.
    /// </summary>
    public interface IRulesEngineOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether automatic creation of rulesets is enabled,
        /// allowing them to be added when a rule is added, when enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if rulesets should be automatically created on rule add; otherwise, <c>false</c>.
        /// </value>
        bool AutoCreateRulesets { get; }

        /// <summary>
        /// Gets the default values for each of the supported data types.
        /// </summary>
        public IDictionary<DataTypes, object> DataTypeDefaults { get; }

        /// <summary>
        /// <para>
        /// Gets or sets the rules engine behavior when no condition with a specific name is
        /// provided to rules engine to match with a rule's condition with the same name.
        /// </para>
        /// <para>
        /// e.g. a rule with a condition "Age" is under evaluation but no condition "Age" was supplied.
        /// </para>
        /// </summary>
        public MissingConditionBehaviors MissingConditionBehavior { get; }

        /// <summary>
        /// Gets the priority criteria to untie when multiples rules are matched.
        /// </summary>
        public PriorityCriterias PriorityCriteria { get; }
    }
}