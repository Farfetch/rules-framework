namespace Rules.Framework.WebUI.Dto
{
    using System;

    internal interface IRuleStatusDtoAnalyzer
    {
        RuleStatusDto AnalyzeStatus(DateTime? dateBegin, DateTime? dateEnd);
    }
}