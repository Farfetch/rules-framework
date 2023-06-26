using Rules.Framework.Rql.Types;

namespace Rules.Framework.Rql.Runtime
{
    internal interface IPropertyGet
    {
        RqlAny GetPropertyValue(RqlString name);

        RqlBool TryGetPropertyValue(RqlString memberName, out RqlAny result);
    }
}