namespace Rules.Framework
{
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
        public PriorityCriterias PriotityCriteria { get; set; }

        /// <summary>
        /// Creates a new set of rules engine options with framework-configured defaults.
        /// </summary>
        /// <remarks>
        /// <para>MissingConditionBehavior = UseDataTypeDefault</para>
        /// <para>PriotityCriteria = TopMostRuleWins</para>
        /// <para>DataTypes.Boolean default = default(bool)</para>
        /// <para>DataTypes.Decimal default = default(decimal)</para>
        /// <para>DataTypes.Integer default = default(int)</para>
        /// <para>DataTypes.String default = string.Empty</para>
        /// </remarks>
        /// <returns></returns>
        public static RulesEngineOptions NewWithDefaults()
        {
            RulesEngineOptions rulesEngineOptions = new RulesEngineOptions
            {
                MissingConditionBehavior = MissingConditionBehaviors.UseDataTypeDefault,
                PriotityCriteria = PriorityCriterias.TopMostRuleWins
            };

            rulesEngineOptions.DataTypeDefaults[DataTypes.Boolean] = default(bool);
            rulesEngineOptions.DataTypeDefaults[DataTypes.Decimal] = default(decimal);
            rulesEngineOptions.DataTypeDefaults[DataTypes.Integer] = default(int);
            rulesEngineOptions.DataTypeDefaults[DataTypes.String] = string.Empty;

            return rulesEngineOptions;
        }
    }
}