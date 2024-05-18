namespace Rules.Framework.IntegrationTests.Common.Scenarios
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework.Generic;

    public interface IScenarioData<TRuleset, TCondition>
    {
        IDictionary<TCondition, object> Conditions { get; }

        DateTime MatchDate { get; }

        IEnumerable<Rule<TRuleset, TCondition>> Rules { get; }
    }
}