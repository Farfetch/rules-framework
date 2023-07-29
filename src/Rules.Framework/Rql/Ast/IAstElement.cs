namespace Rules.Framework.Rql.Ast
{
    internal interface IAstElement
    {
        public RqlSourcePosition BeginPosition { get; }

        public RqlSourcePosition EndPosition { get; }
    }
}