namespace Rules.Framework.Source
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class GetRulesArgs<TContentType>
    {
        public TContentType ContentType { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime DateEnd { get; set; }
    }
}
