namespace Rules.Framework.Extensions
{
    using System;
    using Rules.Framework.Generics;
    using Rules.Framework.Rql;

    internal static class GenericRqlOptionsExtensions
    {
        public static RqlOptions ToRqlOptions(this GenericRqlOptions genericRqlOptions)
        {
            if (genericRqlOptions == null)
            {
                throw new ArgumentNullException(nameof(genericRqlOptions));
            }

            return new RqlOptions
            {
                OutputWriter = genericRqlOptions.OutputWriter,
            };
        }
    }
}