namespace Rules.Framework.Rql.Pipeline.Parse
{
    internal interface IParseStrategy<out TParseOutput>
    {
        TParseOutput Parse(ParseContext parseContext);
    }
}