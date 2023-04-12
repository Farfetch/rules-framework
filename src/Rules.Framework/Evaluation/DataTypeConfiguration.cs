namespace Rules.Framework.Evaluation
{
    using System;
    using Rules.Framework.Core;

    internal sealed class DataTypeConfiguration
    {
        private DataTypeConfiguration()
        {
        }

        public DataTypes DataType { get; private set; }

        public object Default { get; private set; }

        public Type Type { get; private set; }

        public static DataTypeConfiguration Create(DataTypes dataType, Type type, object @default)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return new DataTypeConfiguration
            {
                DataType = dataType,
                Default = @default,
                Type = type,
            };
        }
    }
}