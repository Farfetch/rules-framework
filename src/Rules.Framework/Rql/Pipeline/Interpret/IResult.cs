namespace Rules.Framework.Rql.Pipeline.Interpret
{
    internal interface IResult
    {
        bool HasOutput { get; }

        string Rql { get; }

        bool Success { get; }
    }
}