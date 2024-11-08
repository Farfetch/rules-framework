namespace Rules.Framework.Providers.InMemory.DataModel
{
    using System;
    using System.Collections.Generic;

    internal class RulesetDataModel
    {
        public DateTime Creation { get; set; }

        public string Name { get; set; }

        public List<RuleDataModel> Rules { get; set; }
    }
}