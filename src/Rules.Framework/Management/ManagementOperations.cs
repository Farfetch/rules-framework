namespace Rules.Framework.Management
{
    using System.Collections.Generic;
    using Rules.Framework.Core;
    using Rules.Framework.Source;

    internal static class ManagementOperations
    {
        public static ManagementOperationsSelector<TContentType, TConditionType> Manage<TContentType, TConditionType>(
            IEnumerable<Rule<TContentType, TConditionType>> rules)
            => new ManagementOperationsSelector<TContentType, TConditionType>(rules);

        internal sealed class ManagementOperationsSelector<TContentType, TConditionType>
        {
            private readonly IEnumerable<Rule<TContentType, TConditionType>> rules;

            public ManagementOperationsSelector(IEnumerable<Rule<TContentType, TConditionType>> rules)
            {
                this.rules = rules;
            }

            public ManagementOperationsController<TContentType, TConditionType> UsingSource(IRulesSource<TContentType, TConditionType> rulesDataSource)
                => new ManagementOperationsController<TContentType, TConditionType>(rulesDataSource, this.rules);
        }
    }
}