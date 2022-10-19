namespace Rules.Framework.Evaluation
{
    using System;
    using Rules.Framework.Core;

    internal sealed class DataTypeConfiguration
    {
        private DataTypeConfiguration()
        {
        }

        public DataTypes DataType { get; set; }

        public object Default { get; set; }

        public Type Type { get; set; }

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
                Type = type
            };
        }
    }
}