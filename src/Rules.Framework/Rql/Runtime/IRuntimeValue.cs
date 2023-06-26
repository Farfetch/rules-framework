namespace Rules.Framework.Rql.Runtime
{
    using System;
    using Rules.Framework.Rql.Types;

    internal interface IRuntimeValue
    {
        Type RuntimeType { get; }

        object RuntimeValue { get; }

        RqlType Type { get; }
    }
}