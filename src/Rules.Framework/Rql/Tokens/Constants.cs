namespace Rules.Framework.Rql.Tokens
{
    using System;
    using System.Linq;
    using System.Reflection;

    internal static class Constants
    {
        private static readonly TokenType[] allowedEscapedIdentifierNames;

        private static readonly TokenType[] allowedUnescapedIdentifierNames;

        static Constants()
        {
            var tokenTypeType = typeof(TokenType);
            var allowedEscapedIdentifierMembers = tokenTypeType.GetMembers()
                .Where(mi => mi.GetCustomAttribute<AllowAsIdentifierAttribute>() is not null);
            allowedEscapedIdentifierNames = allowedEscapedIdentifierMembers.Select(mi => Enum.Parse<TokenType>(mi.Name))
                .ToArray();
            allowedUnescapedIdentifierNames = allowedEscapedIdentifierMembers.Where(mi => !mi.GetCustomAttribute<AllowAsIdentifierAttribute>().RequireEscaping)
                .Select(mi => Enum.Parse<TokenType>(mi.Name))
                .ToArray();
        }

        internal static TokenType[] AllowedEscapedIdentifierNames => allowedEscapedIdentifierNames;

        internal static TokenType[] AllowedUnescapedIdentifierNames => allowedUnescapedIdentifierNames;
    }
}