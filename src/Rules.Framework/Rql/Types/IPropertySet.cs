namespace Rules.Framework.Rql.Types
{
    internal interface IPropertySet
    {
        RqlAny this[string name] { set; }
    }
}