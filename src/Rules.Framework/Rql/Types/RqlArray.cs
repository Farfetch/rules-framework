namespace Rules.Framework.Rql.Types
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public readonly struct RqlArray : IRuntimeValue, IPropertyGet
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

        public RqlType Type => type;

        public readonly RqlAny[] Value { get; }

        public RqlAny this[string name]
        {
            get
            {
                if (this.TryGet(name, out var result))
                {
                    return result;
                }

                throw new KeyNotFoundException($"Key with name '{name}' has not been found.");
            }
        }

        public static object[] ConvertToNativeArray(RqlArray rqlArray)
        {
            var result = new object[rqlArray.size];
            for (int i = 0; i < rqlArray.size; i++)
            {
                result[i] = rqlArray.Value[i];
            }

            return result;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder()
                .Append('<')
                .Append(this.Type.Name)
                .Append('>')
                .Append(' ');

            if (this.size > 0)
            {
                stringBuilder.Append("{ ");
                var min = Math.Min(this.size, 5);
                for (int i = 0; i < min; i++)
                {
                    stringBuilder.Append(this.Value[i]);
                    if (i < min - 1)
                    {
                        stringBuilder.Append(", ");
                    }
                }

                if (min < this.size)
                {
                    stringBuilder.Append(", ...");
                }

                stringBuilder.Append(" }");
            }

            return stringBuilder.ToString();
        }

        public bool TryGet(string memberName, out RqlAny result)
        {
            if (string.Equals(memberName, "Size", StringComparison.Ordinal))
            {
                result = new RqlInteger(this.size);
                return true;
            }

            result = new RqlNothing();
            return false;
        }
    }
}