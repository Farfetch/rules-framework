namespace Rules.Framework.Rql
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The data structure that describes a position within' a Rule Query Language source.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct RqlSourcePosition : IEquatable<RqlSourcePosition>
    {
        private RqlSourcePosition(uint line, uint column)
        {
            this.Line = line;
            this.Column = column;
        }

        /// <summary>
        /// The Rule Query Language source position column.
        /// </summary>
        public readonly uint Column;

        /// <summary>
        /// The Rule Query Language source position line.
        /// </summary>
        public readonly uint Line;

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >(RqlSourcePosition left, RqlSourcePosition right)
        {
            if (left.Line < right.Line)
            {
                return false;
            }

            if (left.Line > right.Line)
            {
                return true;
            }

            return left.Column > right.Column;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <(RqlSourcePosition left, RqlSourcePosition right)
        {
            if (left.Line > right.Line)
            {
                return false;
            }

            if (left.Line < right.Line)
            {
                return true;
            }

            return left.Column < right.Column;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator >=(RqlSourcePosition left, RqlSourcePosition right)
        {
            if (left.Line < right.Line)
            {
                return false;
            }

            if (left.Line > right.Line)
            {
                return true;
            }

            return left.Column >= right.Column;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator <=(RqlSourcePosition left, RqlSourcePosition right)
        {
            if (left.Line > right.Line)
            {
                return false;
            }

            if (left.Line < right.Line)
            {
                return true;
            }

            return left.Column <= right.Column;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(RqlSourcePosition left, RqlSourcePosition right) => left.Equals(right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(RqlSourcePosition left, RqlSourcePosition right) => !left.Equals(right);

        /// <summary>
        /// Gets a default empty position.
        /// </summary>
        /// <value>The empty.</value>
        public static RqlSourcePosition Empty { get; } = new RqlSourcePosition(0, 0);

        /// <summary>
        /// Creates a Rule Query Language source position from the given line and column.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public static RqlSourcePosition From(uint line, uint column) => new RqlSourcePosition(line, column);

        /// <inheritdoc/>
        public override string ToString() => $"{{{this.Line}:{this.Column}}}";

        /// <inheritdoc/>
        public bool Equals(RqlSourcePosition other)
            => this.Line == other.Line && this.Column == other.Column;

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj is not RqlSourcePosition)
            {
                return false;
            }

            return this.Equals((RqlSourcePosition)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.Line.GetHashCode() ^ this.Column.GetHashCode();
        }
    }
}