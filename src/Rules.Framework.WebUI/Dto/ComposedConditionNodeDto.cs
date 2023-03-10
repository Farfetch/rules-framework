namespace Rules.Framework.WebUI.Dto
{
    using System.Collections.Generic;

    internal sealed class ComposedConditionNodeDto : ConditionNodeDto
    {
        public IEnumerable<ConditionNodeDto> ChildConditionNodes { get; internal set; }
    }
}