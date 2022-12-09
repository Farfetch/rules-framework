namespace Rules.Framework.Evaluation
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Auto)]
    internal struct EvaluationOptions : IEquatable<EvaluationOptions>
    {
        public bool ExcludeRulesWithoutSearchConditions { get; set; }

        public MatchModes MatchMode { get; set; }

        public override bool Equals(object obj)
            => obj is EvaluationOptions options && this.Equals(options);

        public bool Equals(EvaluationOptions other)
            => this.ExcludeRulesWithoutSearchConditions == other.ExcludeRulesWithoutSearchConditions
                && this.MatchMode == other.MatchMode;

        public override int GetHashCode()
        {
            int hashCode = -1260975221;
            hashCode = hashCode * -1521134295 + this.ExcludeRulesWithoutSearchConditions.GetHashCode();
            hashCode = hashCode * -1521134295 + this.MatchMode.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(EvaluationOptions left, EvaluationOptions right) => left.Equals(right);

        public static bool operator !=(EvaluationOptions left, EvaluationOptions right) => !(left == right);
    }
}