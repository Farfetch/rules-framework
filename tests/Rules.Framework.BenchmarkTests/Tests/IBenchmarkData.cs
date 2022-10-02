namespace Rules.Framework.BenchmarkTests.Tests
{
    using Rules.Framework;
    using Rules.Framework.BenchmarkTests.Tests.Benchmark1;
    using Rules.Framework.Core;
    using System;
    using System.Collections.Generic;

    internal interface IBenchmarkData<TContentType, TConditionType>
    {
        IEnumerable<Condition<TConditionType>> Conditions { get; }
        DateTime MatchDate { get; }
        IEnumerable<Rule<TContentType, TConditionType>> Rules { get; }
    }
}