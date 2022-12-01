namespace Rules.Framework.Evaluation
{
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
                    this.leftSupportForOneMultiplicity = this.SupportedMultiplicities?.Any(m => m.Contains("one-to")) ?? false;
                }

                return this.leftSupportForOneMultiplicity.GetValueOrDefault();
            }
        }

        public Operators Operator { get; set; }

        public string[] SupportedMultiplicities { get; set; }

        public IEnumerable<string> GetAllCombinations() => this.SupportedMultiplicities.Select(x => $"{x}-{this.Operator}");
    }
}