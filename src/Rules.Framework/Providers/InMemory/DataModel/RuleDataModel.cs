namespace Rules.Framework.Providers.InMemory.DataModel
{
    using System;

    internal sealed class RuleDataModel<TContentType, TConditionType>
    {
        public bool Active { get; set; } = true;

        public dynamic Content { get; set; }

        public TContentType ContentType { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public string Name { get; set; }

        public int Priority { get; set; }

        public ConditionNodeDataModel<TConditionType> RootCondition { get; set; }
    }
}
