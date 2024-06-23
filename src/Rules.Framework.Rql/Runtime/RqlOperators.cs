namespace Rules.Framework.Rql.Runtime
{
    internal enum RqlOperators
    {
        None = 0,
        Plus = 1,
        Minus = 2,
        Star = 3,
        Slash = 4,
        Mod = 5,
        And = 6,
        Or = 7,
        Equals = 8,
        NotEquals = 9,
        GreaterThan = 10,
        GreaterThanOrEquals = 11,
        LesserThan = 12,
        LesserThanOrEquals = 13,
        In = 14,
        NotIn = 15,
        Assign = 16,
    }
}