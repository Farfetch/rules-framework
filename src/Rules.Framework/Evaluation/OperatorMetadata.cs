namespace Rules.Framework.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rules.Framework.Core;

    internal sealed class OperatorMetadata
    {
        private bool? leftSupportForOneMultiplicity = null;

        public bool HasSupportForOneMultiplicityAtLeft
        {
            get
            {
                if (this.leftSupportForOneMultiplicity is null)
                {
#if NETSTANDARD2_0
                    this.leftSupportForOneMultiplicity = this.SupportedMultiplicities?.Any(m => m.Contains("one-to")) ?? false;
#else
                    this.leftSupportForOneMultiplicity = this.SupportedMultiplicities?.Any(m => m.Contains("one-to", StringComparison.Ordinal)) ?? false;
#endif
                }

                return this.leftSupportForOneMultiplicity.GetValueOrDefault();
            }
        }

        public Operators Operator { get; set; }

        public string[] SupportedMultiplicities { get; set; } = Array.Empty<string>();

        public IEnumerable<string> GetAllCombinations() => this.SupportedMultiplicities.Select(x => $"{x}-{this.Operator}");
    }
}