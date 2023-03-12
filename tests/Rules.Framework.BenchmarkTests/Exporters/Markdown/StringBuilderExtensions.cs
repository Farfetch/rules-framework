namespace Rules.Framework.BenchmarkTests.Exporters.Markdown
{
    using System;
    using System.Text;

    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendIf(this StringBuilder builder, string text, Func<bool> conditionFunc)
        {
            if (conditionFunc.Invoke())
            {
                return builder.Append(text);
            }

            return builder;
        }
    }
}