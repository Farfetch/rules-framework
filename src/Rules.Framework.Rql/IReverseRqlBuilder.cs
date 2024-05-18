namespace Rules.Framework.Rql
{
    using Rules.Framework.Rql.Ast;

    internal interface IReverseRqlBuilder
    {
        string BuildRql(IAstElement astElement);
    }
}