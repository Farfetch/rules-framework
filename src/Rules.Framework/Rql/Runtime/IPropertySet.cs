using Rules.Framework.Rql.Runtime.Types;

namespace Rules.Framework.Rql.Runtime
{
    internal interface IPropertySet
    {
        RqlAny SetPropertyValue(RqlString name, RqlAny value);
    }
}