namespace Rules.Framework.Rql.Ast
{
    internal interface IAstElement
    {
        RqlSourcePosition BeginPosition { get; }

        RqlSourcePosition EndPosition { get; }

        bool ContainsPosition(RqlSourcePosition position);
    }
}