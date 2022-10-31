namespace Rules.Framework.WebApi.Controllers
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Rules.Framework.WebApi.Response;

    public class RuleController : Controller
    {
        private const string dateFormat = "dd/MM/yyyy HH:mm:ss";
        private readonly JsonSerializerSettings jsonSerializerSettings;
        private readonly IRulesEngine rulesEngine;

        public RuleController(IRulesEngine rulesEngine)
        {
            this.rulesEngine = rulesEngine;
            this.jsonSerializerSettings = new JsonSerializerSettings();
            this.jsonSerializerSettings.Converters.Add(new StringEnumConverter());
        }

        [Route("rules/{controller}/priority")]
        public async Task<IActionResult> GetPriorityCriterias()
        {
            var priorityOption = this.rulesEngine
                .GetPriorityCriterias();

            return Ok(priorityOption.ToString());
        }

        [Route("rules/{controller}/{conditionType}/list")]
        public async Task<IActionResult> List([FromRoute] string conditionType)
        {
            var list = new List<RuleDto>();

            var rules = await this.rulesEngine
                .SearchAsync(new SearchArgs<ContentType, ConditionType>(new ContentType
                {
                    Name = conditionType
                }, DateTime.MinValue, DateTime.MaxValue));

            var priorityOption = this.rulesEngine
               .GetPriorityCriterias();

            if (priorityOption == PriorityCriterias.BottommostRuleWins)
            {
                rules = rules.OrderByDescending(r => r.Priority);
            }
            else
            {
                rules = rules.OrderBy(r => r.Priority);
            }

            if (rules != null)
            {
                foreach (var rule in rules)
                {                    
                    list.Add(new RuleDto
                    {
                        Priority = rule.Priority,
                        Name = rule.Name,
                        Value = JsonConvert.SerializeObject(rule.ContentContainer, jsonSerializerSettings),
                        DateEnd = !rule.DateEnd.HasValue ? "-" : rule.DateEnd.Value.ToString(dateFormat),
                        DateBegin = rule.DateBegin.ToString(dateFormat),
                        Status = GetRuleStatus(rule.DateBegin, rule.DateEnd).ToString(),
                        Conditions = rule.RootCondition is null ? string.Empty : JsonConvert.SerializeObject(rule.RootCondition, jsonSerializerSettings)
                    });
                }
            }

            return Ok(list);
        }

        private static RuleStatusDto GetRuleStatus(DateTime? dateBegin, DateTime? dateEnd)
        {
            if (!dateBegin.HasValue)
            {
                return RuleStatusDto.Inactive;
            }

            if (dateBegin.Value > DateTime.UtcNow)
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