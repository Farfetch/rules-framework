namespace Rules.Framework.Rql.Tokens
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    internal class AllowAsIdentifierAttribute : Attribute
    {
        public bool RequireEscaping { get; set; }
    }
}