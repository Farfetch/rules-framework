namespace Rules.Framework
{
    using System.Collections.Generic;

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
        /// Gets or sets a value indicating whether automatic creation of content types is enabled,
        /// allowing them to be added when a rule is added, when enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if content types should be automatically created on rule add; otherwise, <c>false</c>.
        /// </value>
        public bool AutoCreateContentTypes { get; set; }

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
                AutoCreateContentTypes = false,
            };

            return rulesEngineOptions;
        }
    }
}