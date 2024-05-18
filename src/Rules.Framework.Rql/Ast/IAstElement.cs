namespace Rules.Framework.Rql.Ast
{
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Rql.Tokens;

    internal interface IAstElement
    {
        RqlSourcePosition BeginPosition { get; }

        RqlSourcePosition EndPosition { get; }
    }
}