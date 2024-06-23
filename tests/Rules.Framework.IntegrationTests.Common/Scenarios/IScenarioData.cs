namespace Rules.Framework.IntegrationTests.Common.Scenarios
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.Core;

    public interface IScenarioData<TContentType, TConditionType>
    {
        IEnumerable<Condition<TConditionType>> Conditions { get; }

        DateTime MatchDate { get; }

        IEnumerable<Rule<TContentType, TConditionType>> Rules { get; }
    }
}