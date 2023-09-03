namespace Rules.Framework.Rql.Pipeline.Parse
{
    using System.Collections.Generic;
    using Rules.Framework.Rql.Tokens;

    internal interface IParser
    {
        ParseResult Parse(IReadOnlyList<Token> tokens);
    }
}