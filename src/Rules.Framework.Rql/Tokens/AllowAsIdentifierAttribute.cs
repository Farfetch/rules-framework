namespace Rules.Framework.Rql.Tokens
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [AttributeUsage(AttributeTargets.Field)]
    [ExcludeFromCodeCoverage]
    internal class AllowAsIdentifierAttribute : Attribute
    {
        public bool RequireEscaping { get; set; }
    }
}