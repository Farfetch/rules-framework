namespace Rules.Framework.WebUI.Dto
{
    using System;

    internal interface IRuleStatusDtoAnalyzer
    {
        RuleStatusDto Analyze(DateTime? dateBegin, DateTime? dateEnd);
    }
}