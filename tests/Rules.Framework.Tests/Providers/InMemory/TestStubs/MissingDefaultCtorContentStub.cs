namespace Rules.Framework.Tests.Providers.InMemory.TestStubs
{
    internal class MissingDefaultCtorContentStub
    {
        public MissingDefaultCtorContentStub(int prop1)
        {
            this.Prop1 = prop1;
        }

        public int Prop1 { get; set; }
    }
}