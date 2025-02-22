namespace Rules.Framework.Rql
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents the RQL options available for customization of a RQL Engine instance.
    /// </summary>
    public class RqlOptions
    {
        /// <summary>
        /// Gets or sets the output writer.
        /// </summary>
        /// <value>The output writer.</value>
        public TextWriter OutputWriter { get; set; }

        /// <summary>
        /// Creates a new RQL options with the default options.
        /// </summary>
        /// <returns></returns>
        public static RqlOptions NewWithDefaults()
        {
            return new RqlOptions
            {
                OutputWriter = Console.Out,
            };
        }
    }
}