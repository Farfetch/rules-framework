namespace Rules.Framework.Rql.Pipeline.Interpret
{
    internal interface IResult
    {
        bool HasOutput { get; }

        bool Success { get; }
    }
}