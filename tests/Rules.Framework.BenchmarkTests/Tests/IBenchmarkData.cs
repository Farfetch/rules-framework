namespace Rules.Framework.BenchmarkTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Rules.Framework;
    using Rules.Framework.Core;

    internal interface IBenchmarkData<TContentType, TConditionType>
    {
        IEnumerable<Condition<TConditionType>> Conditions { get; }

        DateTime MatchDate { get; }

        IEnumerable<Rule<TContentType, TConditionType>> Rules { get; }
    }
}