using System.Collections.Generic;
using Rules.Framework.Core;

namespace Rules.Framework
{
    public class RulesEngineOptions
    {
        private RulesEngineOptions()
        {
            this.DataTypeDefaults = new Dictionary<DataTypes, object>();
        }

        public static RulesEngineOptions Default
        {
            get
            {
                RulesEngineOptions rulesEngineOptions = new RulesEngineOptions
                {
                    MissingConditionBehavior = MissingConditionBehaviors.UseDataTypeDefault,
                    PriotityCriteria = PriorityCriterias.TopmostRuleWins
                };

                rulesEngineOptions.DataTypeDefaults[DataTypes.Boolean] = default(bool);
                rulesEngineOptions.DataTypeDefaults[DataTypes.Decimal] = default(decimal);
                rulesEngineOptions.DataTypeDefaults[DataTypes.Integer] = default(int);
                rulesEngineOptions.DataTypeDefaults[DataTypes.String] = string.Empty;

                return rulesEngineOptions;
            }
        }

        public IDictionary<DataTypes, object> DataTypeDefaults { get; }

        public MissingConditionBehaviors MissingConditionBehavior { get; set; }

        public PriorityCriterias PriotityCriteria { get; set; }
    }
}