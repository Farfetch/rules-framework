namespace Rules.Framework.Rql.Pipeline.Scan
{
    internal interface IScanner
    {
        ScanResult ScanTokens(string source);
    }
}