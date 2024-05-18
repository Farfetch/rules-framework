namespace Rules.Framework.Rql.Tests.TestStubs
{
    using System;
    using Rules.Framework.Rql.Pipeline.Interpret;

    internal class StubResult : IResult
    {
        public bool HasOutput => throw new NotImplementedException();

        public string Rql => throw new NotImplementedException();

        public bool Success => true;
    }
}