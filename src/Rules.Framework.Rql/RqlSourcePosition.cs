namespace Rules.Framework.Rql
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct RqlSourcePosition : IEquatable<RqlSourcePosition>
    {
        private RqlSourcePosition(uint line, uint column)
        {
            this.Line = line;
            this.Column = column;
        }

        public readonly uint Column;

        public readonly uint Line;

        public static RqlSourcePosition Empty { get; } = new RqlSourcePosition(0, 0);

        public static RqlSourcePosition From(uint line, uint column) => new RqlSourcePosition(line, column);

        public override string ToString() => $"{{{this.Line}:{this.Column}}}";

        public bool Equals(RqlSourcePosition other)
            => this.Line == other.Line && this.Column == other.Column;
    }
}