namespace Rules.Framework.Providers.SqlServer
{
    using System;

    public class Script
    {
        public Script(string value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value { get; set; }
    }
}