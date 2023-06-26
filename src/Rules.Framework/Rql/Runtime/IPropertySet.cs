using Rules.Framework.Rql.Types;

namespace Rules.Framework.Rql.Runtime
{
    internal interface IPropertySet
    {
        RqlAny SetPropertyValue(RqlString name, RqlAny value);
    }
}