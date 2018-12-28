namespace Rules.Framework
{
    public class RulesEngineOptions
    {
        public static RulesEngineOptions Default => new RulesEngineOptions
        {
            PriotityCriteria = PriorityCriterias.TopmostRuleWins
        };

        public PriorityCriterias PriotityCriteria { get; set; }
    }
}