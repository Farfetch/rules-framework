namespace Rules.Framework.WebUI.Dto
{
    using System;

    internal sealed class RuleStatusDtoAnalyzer : IRuleStatusDtoAnalyzer
    {
        public RuleStatusDto Analyze(DateTime dateBegin, DateTime? dateEnd)
        {
            if (dateBegin > DateTime.UtcNow)
            {
                return RuleStatusDto.Pending;
            }

            if (!dateEnd.HasValue)
            {
                return RuleStatusDto.Active;
            }

            if (dateEnd.Value <= DateTime.UtcNow)
            {
                return RuleStatusDto.Inactive;
            }

            return RuleStatusDto.Active;
        }
    }
}