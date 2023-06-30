namespace Rules.Framework.Rql.Runtime.Types
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Rules.Framework.Rql.Runtime;

    public readonly struct RqlArray : IRuntimeValue, IPropertyGet, IIndexerGet, IIndexerSet
    {
        private static readonly Type runtimeType = typeof(object[]);
        private static readonly RqlType type = RqlTypes.Array;
        private readonly int size;

        public RqlArray(int size)
        {
            this.size = size;
            this.Value = new RqlAny[size];
        }

        public Type RuntimeType => runtimeType;

        public object RuntimeValue => ConvertToNativeArray(this);

        public RqlInteger Size => this.size;

        public RqlType Type => type;

        public readonly RqlAny[] Value { get; }

        public static object[] ConvertToNativeArray(RqlArray rqlArray)
        {
            var result = new object[rqlArray.size];
            for (int i = 0; i < rqlArray.size; i++)
            {
                result[i] = rqlArray.Value[i];
            }

            return result;
        }

        public static implicit operator RqlAny(RqlArray rqlArray) => new RqlAny(rqlArray);

        public RqlAny GetAtIndex(RqlInteger index)
        {
            if (index.Value < 0 || index.Value >= this.size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"The value of '{index}' is out of the '{nameof(RqlArray)}' range.");
            }

            return this.Value[index.Value];
        }

        public RqlAny GetPropertyValue(RqlString name)
        {
            if (this.TryGetPropertyValue(name, out var result))
            {
                return result;
            }

            throw new KeyNotFoundException($"Key with name '{name.Value}' has not been found.");
        }

        public RqlNothing SetAtIndex(RqlInteger index, RqlAny value)
        {
            if (index.Value < 0 || index.Value >= this.size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"The value of '{index}' is out of the '{nameof(RqlArray)}' range.");
            }

            this.Value[index.Value] = value;
            return new RqlNothing();
        }

        public override string ToString()
            => this.ToString(0);

        public RqlBool TryGetPropertyValue(RqlString memberName, out RqlAny result)
        {
            if (string.Equals(memberName, "Size", StringComparison.Ordinal))
            {
                result = new RqlInteger(this.size);
                return true;
            }

            result = new RqlNothing();
            return false;
        }

        internal string ToString(int indent)
        {
            var stringBuilder = new StringBuilder()
                .Append('<')
                .Append(this.Type.Name)
                .Append('>')
                .Append(' ');

            if (this.size > 0)
            {
                stringBuilder.AppendLine()
                    .Append(new string(' ', indent))
                    .Append("{")
                    .AppendLine();
                var min = Math.Min(this.size, 5);
                for (int i = 0; i < min; i++)
                {
                    stringBuilder.Append(new string(' ', indent + 4))
                        .Append(this.Value[i]);
                    if (i < min - 1)
                    {
                        stringBuilder.Append(',')
                            .AppendLine();
                    }
                }

                if (min < this.size)
                {
                    stringBuilder.Append(',')
                        .AppendLine()
                        .Append(new string(' ', indent + 4))
                        .Append("...");
                }

                stringBuilder.AppendLine()
                    .Append(new string(' ', indent))
                    .Append("}");
            }

            return stringBuilder.ToString();
        }
    }
}