namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;

    public readonly struct RqlRule<TContentType, TConditionType> : IRuntimeValue, IEquatable<RqlRule<TContentType, TConditionType>>
    {
        private static readonly Type runtimeType = typeof(Rule<TContentType, TConditionType>);
        private readonly Dictionary<string, RqlAny> properties;

        internal RqlRule(Rule<TContentType, TConditionType> rule)
        {
            this.Value = rule;
            this.properties = new Dictionary<string, RqlAny>(StringComparer.Ordinal)
            {
                { "Active", new RqlBool(rule.Active) },
                { "DateBegin", new RqlDate(rule.DateBegin) },
                { "DateEnd", rule.DateEnd.HasValue ? new RqlDate(rule.DateEnd.Value) : new RqlNothing() },
                { "Name", new RqlString(rule.Name) },
                { "Priority", new RqlInteger(rule.Priority) },
                { "RootCondition", rule.RootCondition is not null ? ConvertCondition(rule.RootCondition) : new RqlNothing() },
            };
        }

        public Type RuntimeType => runtimeType;

        public object RuntimeValue => this.Value;

        public RqlType Type => RqlTypes.Rule;

        public readonly Rule<TContentType, TConditionType> Value { get; }

        public static implicit operator RqlAny(RqlRule<TContentType, TConditionType> rqlRule) => new RqlAny(rqlRule);

        public bool Equals(RqlRule<TContentType, TConditionType> other) => this.Value.Equals(other.Value);

        public override string ToString()
                    => $"<{Type.Name}>{Environment.NewLine}{this.ToString(4)}";

        internal string ToString(int indent)
        {
            var stringBuilder = new StringBuilder()
                .Append('{');

            foreach (var property in this.properties)
            {
                stringBuilder.AppendLine()
                    .Append(new string(' ', indent))
                    .Append(property.Key)
                    .Append(": ");

                if (property.Value.UnderlyingType == RqlTypes.Object)
                {
                    stringBuilder.Append(property.Value.Unwrap<RqlObject>().ToString(indent + 4));
                    continue;
                }

                if (property.Value.UnderlyingType == RqlTypes.ReadOnlyObject)
                {
                    stringBuilder.Append(property.Value.Unwrap<RqlReadOnlyObject>().ToString(indent + 4));
                    continue;
                }

                if (property.Value.UnderlyingType == RqlTypes.Array)
                {
                    stringBuilder.Append(property.Value.Unwrap<RqlArray>().ToString());
                    continue;
                }

                stringBuilder.Append(property.Value.Value);
            }

            return stringBuilder.AppendLine()
                .Append(new string(' ', indent - 4))
                .Append('}')
                .ToString();
        }

        private static RqlAny ConvertCondition(IConditionNode<TConditionType> condition)
        {
            switch (condition)
            {
                case ComposedConditionNode<TConditionType> ccn:
                    var childConditions = new RqlArray(ccn.ChildConditionNodes.Count());
                    var i = 0;
                    foreach (var childConditionNode in ccn.ChildConditionNodes)
                    {
                        childConditions.SetAtIndex(i++, ConvertCondition(childConditionNode));
                    }

                    var composedConditionProperties = new Dictionary<string, RqlAny>(StringComparer.Ordinal)
                    {
                        { "ChildConditionNodes", childConditions },
                        { "LogicalOperator", new RqlString(ccn.LogicalOperator.ToString()) },
                    };
                    return new RqlReadOnlyObject(composedConditionProperties);

                case ValueConditionNode<TConditionType> vcn:
                    var valueConditionProperties = new Dictionary<string, RqlAny>(StringComparer.Ordinal)
                    {
                        { "ConditionType", new RqlString(vcn.ConditionType.ToString()) },
                        { "DataType", new RqlString(vcn.DataType.ToString()) },
                        { "LogicalOperator", new RqlString(vcn.LogicalOperator.ToString()) },
                        { "Operand", ConvertValue(vcn.Operand) },
                        { "Operator", new RqlString(vcn.Operator.ToString()) },
                    };
                    return new RqlReadOnlyObject(valueConditionProperties);

                default:
                    throw new NotSupportedException($"Specified condition node type is not supported: {condition.GetType().FullName}");
            }
        }

        private static RqlAny ConvertValue(object value)
        {
            return value switch
            {
                IEnumerable<int> intArray => CreateArray(intArray),
                IEnumerable<decimal> decimalArray => CreateArray(decimalArray),
                IEnumerable<bool> boolArray => CreateArray(boolArray),
                IEnumerable<string> stringArray => CreateArray(stringArray),
                int i => new RqlInteger(i),
                decimal d => new RqlDecimal(d),
                bool b => new RqlBool(b),
                string s => new RqlString(s),
                null => new RqlNothing(),
                _ => throw new NotSupportedException($"Specified value is not supported for conversion to RQL type system: {value.GetType().FullName}"),
            };
        }

        private static RqlArray CreateArray<T>(IEnumerable<T> source)
        {
            var count = source.Count();
            var rqlArray = new RqlArray(count);
            for (var i = 0; i < count; i++)
            {
                rqlArray.SetAtIndex(i, ConvertValue(source.ElementAt(i)!));
            }

            return rqlArray;
        }
    }
}