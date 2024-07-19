namespace Rules.Framework.Providers.InMemory.DataModel
{
    using System;

    internal sealed class RuleDataModel
    {
        public bool Active { get; set; } = true;

        public dynamic Content { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public string Name { get; set; }

        public int Priority { get; set; }

        public ConditionNodeDataModel RootCondition { get; set; }

        public string Ruleset { get; set; }
    }
}