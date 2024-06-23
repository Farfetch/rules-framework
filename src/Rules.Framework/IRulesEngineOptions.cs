namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;

    /// <summary>
    /// The set of rules engine options that influence rules engine rules matching.
    /// </summary>
    public interface IRulesEngineOptions
    {
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