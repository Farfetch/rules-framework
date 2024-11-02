namespace Rules.Framework.Source
{
    using System;

    internal sealed class GetRulesArgs
    {
        public DateTime DateBegin { get; set; }

        public DateTime DateEnd { get; set; }

        public string Ruleset { get; set; }
    }
}