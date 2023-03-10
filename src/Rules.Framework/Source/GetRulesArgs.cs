namespace Rules.Framework.Source
{
    using System;

    internal sealed class GetRulesArgs<TContentType>
    {
        public TContentType ContentType { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime DateEnd { get; set; }
    }
}