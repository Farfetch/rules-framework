namespace Rules.Framework.Providers.MongoDb.Tests.TestStubs
{
    using System;

    internal class ContentStub
    {
        // For the purpose of testing when type is already int.
        public int Prop01 { get; set; }

        // For the purpose of testing when type is string.
        public string Prop02 { get; set; }

        // For the purpose of testing when type is already decimal.
        public decimal Prop03 { get; set; }

        // For the purpose of testing when type must be parsed to Guid.
        public Guid Prop04 { get; set; }

        // For the purpose of testing when type must be parsed to specific enum.
        public ContentType Prop05 { get; set; }

        // For the purpose of testing when type must be parsed to int.
        public int Prop06 { get; set; }

        // For the purpose of testing when type must be parsed to decimal.
        public decimal Prop07 { get; set; }

        // For the purpose of testing when type is already bool.
        public bool Prop08 { get; set; }

        // For the purpose of testing when type must be parsed to bool.
        public bool Prop09 { get; set; }

        // For the purpose of testing when type is already DateTime.
        public DateTime Prop10 { get; set; }

        // For the purpose of testing when type must be parsed to DateTime.
        public DateTime Prop11 { get; set; }
    }
}