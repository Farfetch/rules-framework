namespace Rules.Framework.Rql.Pipeline.Parse
{
    internal interface IParseStrategy<TParseOutput>
    {
        TParseOutput Parse(ParseContext parseContext);
    }
}