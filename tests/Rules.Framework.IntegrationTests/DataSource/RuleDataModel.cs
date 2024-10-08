namespace Rules.Framework.IntegrationTests.DataSource
{
    using System;

    internal class RuleDataModel
    {
        public string Content { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public ConditionNodeDataModel RootCondition { get; set; }
        public string Ruleset { get; set; }
    }
}