namespace Rules.Framework.WebUI.ViewModels
{
    using System;

    internal class RuleViewModel
    {
        public bool Active { get; set; }

        public string Conditions { get; set; }

        public object Content { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Priority { get; set; }

        public ConditionNodeViewModel RootCondition { get; set; }

        public string Ruleset { get; set; }
    }
}