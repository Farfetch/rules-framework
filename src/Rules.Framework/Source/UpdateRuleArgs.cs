namespace Rules.Framework.Source
{
    using Rules.Framework.Core;
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class UpdateRuleArgs<TContentType, TConditionType>
    {
        public Rule<TContentType, TConditionType> Rule { get; set; }
    }
}
