namespace Rules.Framework.Rql.Types
{
    internal interface IPropertyGet
    {
        RqlAny this[string name] { get; }

        bool TryGet(string memberName, out RqlAny result);
    }
}