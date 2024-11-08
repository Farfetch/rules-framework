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

        /// <inheritdoc/>
        public bool AutoCreateRulesets { get; set; }

        /// <inheritdoc/>
        public IDictionary<DataTypes, object> DataTypeDefaults { get; }

        /// <inheritdoc/>
        public bool EnableCompilation { get; set; }

        /// <inheritdoc/>
        public MissingConditionBehaviors MissingConditionBehavior { get; set; }

        /// <inheritdoc/>
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
                AutoCreateRulesets = false,
            };

            return rulesEngineOptions;
        }
    }
}