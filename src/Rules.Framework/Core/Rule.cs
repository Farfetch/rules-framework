namespace Rules.Framework.Core
{
    using System;

    public class Rule<TContentType, TConditionType>
    {
        public ContentContainer<TContentType> ContentContainer { get; internal set; }

        public DateTime DateBegin { get; internal set; }

        public DateTime? DateEnd { get; internal set; }

        public string Name { get; internal set; }

        public int Priority { get; internal set; }

        public IConditionNode<TConditionType> RootCondition { get; internal set; }
    }
}