namespace Rules.Framework.Admin.WebApi.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Rules.Framework.Admin.WebApi.Response;

    public class RuleController : Controller
    {
        private readonly IRulesService rulesService;

        public RuleController(IRulesService rulesService)
        {
            this.rulesService = rulesService;
        }

        [Route("rules/{controller}/{conditionType}/list")]
        public async Task<IActionResult> List([FromRoute] string conditionType)
        {
            var list = new List<RuleDto>();

            var rules = await this.rulesService.FindRulesAsync(conditionType, DateTime.UtcNow);
            var jsonRules = JsonConvert.SerializeObject(rules);
            var concreteRules = JsonConvert.DeserializeObject<List<Root>>(jsonRules)?.OrderBy(d => d.Priority);

            if (concreteRules != null)
            {
                foreach (var rule in concreteRules)
                {
                    list.Add(new RuleDto
                    {
                        Id = rule.Priority,
                        Name = rule.Name,
                        Value = rule.ContentContainer.ToString(),
                        DateEnd = rule.DateEnd.HasValue ? rule.DateEnd.Value.ToString("dd/MM/yyyy HH:mm:ss") : "-",
                        DateStart = rule.DateBegin.ToString("dd/MM/yyyy HH:mm:ss"),
                        Active = true,
                        Conditions = new List<ConditionDto>(),
                        ConditionsAsJson = rule.RootCondition is null ? string.Empty : JsonConvert.SerializeObject(rule.RootCondition),
                        ConditionsAsTree = rule.RootCondition is null ? string.Empty : JsonConvert.SerializeObject(rule.RootCondition)
                    });
                }
            }

            return Ok(list);
        }
    }
}