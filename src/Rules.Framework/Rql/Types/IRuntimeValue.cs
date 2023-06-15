namespace Rules.Framework.Rql.Types
{
    using System;

    internal interface IRuntimeValue
    {
        Type RuntimeType { get; }

        object RuntimeValue { get; }

        RqlType Type { get; }
    }
}