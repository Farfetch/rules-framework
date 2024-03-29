namespace Rules.Framework
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Core;

    /// <summary>
    /// The set of rules engine options that influence rules engine rules matching.
    /// </summary>
    public class RulesEngineOptions : IRulesEngineOptions
    {
        private RulesEngineOptions()
        {
            this.DataTypeDefaults = new Dictionary<DataTypes, object>();
        }

        /// <summary>
        /// Gets the default values for each of the supported data types.
        /// </summary>
        public IDictionary<DataTypes, object> DataTypeDefaults { get; }

        /// <summary>
        /// Gets or sets whether rules' conditions is enabled or not.
        /// </summary>
        public bool EnableCompilation { get; set; }

        /// <summary>
        /// <para>
        /// Gets or sets the rules engine behavior when no condition with a specific type is
        /// provided to rules engine to match with a rule's condition with the same type.
        /// </para>
        /// <para>
        /// e.g. a rule with a condition of type "Age" is under evaluation but no condition of type
        /// "Age" was supplied.
        /// </para>
        /// </summary>
        public MissingConditionBehaviors MissingConditionBehavior { get; set; }

        /// <summary>
        /// Gets or sets the priority criteria to untie when multiples rules are matched.
        /// </summary>
        public PriorityCriterias PriorityCriteria { get; set; }

        /// <summary>
        /// Gets or sets the priority criteria to untie when multiples rules are matched.
        /// </summary>
        [Obsolete("This property has a typo and has been replaced by PriorityCriteria.")]
        public PriorityCriterias PriotityCriteria
        {
            get { return this.PriorityCriteria; }
            set { this.PriorityCriteria = value; }
        }

        /// <summary>
        /// Creates a new set of rules engine options with framework-configured defaults.
        /// </summary>
        /// <remarks>
        /// <para>MissingConditionBehavior = UseDataTypeDefault</para>
        /// <para>PriorityCriteria = TopmostRuleWins</para>
        /// <para>DataTypes.Boolean default = default(bool)</para>
        /// <para>DataTypes.Decimal default = default(decimal)</para>
        /// <para>DataTypes.Integer default = default(int)</para>
        /// <para>DataTypes.String default = string.Empty</para>
        /// </remarks>
        /// <returns></returns>
        public static RulesEngineOptions NewWithDefaults()
        {
            RulesEngineOptions rulesEngineOptions = new()
            {
                MissingConditionBehavior = MissingConditionBehaviors.UseDataTypeDefault,
                PriorityCriteria = PriorityCriterias.TopmostRuleWins,
                DataTypeDefaults =
                    {
                        [DataTypes.Boolean] = default(bool),
                        [DataTypes.Decimal] = default(decimal),
                        [DataTypes.Integer] = default(int),
                        [DataTypes.String] = string.Empty,
                        [DataTypes.ArrayBoolean] = default(bool),
                        [DataTypes.ArrayDecimal] = default(decimal),
                        [DataTypes.ArrayInteger] = default(int),
                        [DataTypes.ArrayString] = string.Empty,
                    },
            };


            return rulesEngineOptions;
        }
    }
}