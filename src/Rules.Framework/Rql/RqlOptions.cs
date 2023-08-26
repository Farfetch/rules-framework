namespace Rules.Framework.Rql
{
    using System;
    using System.IO;

    public class RqlOptions
    {
        public TextWriter OutputWriter { get; set; }

        public static RqlOptions NewWithDefaults()
        {
            return new RqlOptions
            {
                OutputWriter = Console.Out,
            };
        }
    }
}