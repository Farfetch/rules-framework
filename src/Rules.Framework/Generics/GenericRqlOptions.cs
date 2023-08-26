namespace Rules.Framework.Generics
{
    using System;
    using System.IO;

    public class GenericRqlOptions
    {
        public TextWriter OutputWriter { get; set; }

        public static GenericRqlOptions NewWithDefaults()
        {
            return new GenericRqlOptions
            {
                OutputWriter = Console.Out,
            };
        }
    }
}