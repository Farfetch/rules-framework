namespace Rules.Framework.UI.Sample
{
    public class RulesService
    {
        private readonly RulesEngineProvider rulesEngineProvider;

        public RulesService(RulesEngineProvider rulesEngineProvider)
        {
            this.rulesEngineProvider = rulesEngineProvider;
        }
    }
}