namespace Rules.Framework.Admin.WebApi.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.VisualBasic;
    using Newtonsoft.Json;
    using Rules.Framework.Admin.WebApi.Response;

    public class RuleController : Controller
    {
        private const string dateFormat = "dd/MM/yyyy HH:mm:ss";
        private readonly JsonSerializerSettings jsonSerializerSettings;
        private readonly IRulesService rulesService;

        public RuleController(IRulesService rulesService)
        {
            this.rulesService = rulesService;

            this.jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        }

        public static object? GetProperty(object target, string name, CallType method = CallType.Get)
        {
            return Microsoft.VisualBasic.CompilerServices.Versioned.CallByName(target, name, method);
        }

        [Route("rules/{controller}/options")]
        public async Task<IActionResult> GetOptions()
        {
            var priorityOption = await this.rulesService
                .GetRulePriorityOptionAsync();

            return Ok(priorityOption);
        }

        [Route("rules/{controller}/{conditionType}/list")]
        public async Task<IActionResult> List([FromRoute] string conditionType)
        {
            var list = new List<RuleDto>();

            var rules = await this.rulesService
                .FindRulesAsync(conditionType);

            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    var priority = GetProperty(rule, "Priority") as int?;
                    var name = GetProperty(rule, "Name") as string;
                    var contentContainer = GetProperty(rule, "ContentContainer");
                    var dateEnd = GetProperty(rule, "DateEnd") as DateTime?;
                    var dateBegin = GetProperty(rule, "DateBegin") as DateTime?;
                    var conditions = GetProperty(rule, "RootCondition") as string;

                    list.Add(new RuleDto
                    {
                        Priority = priority.HasValue ? priority.Value : 0,
                        Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name,
                        Value = contentContainer is null ? "" : JsonConvert.SerializeObject(contentContainer.GetContentAs<dynamic>(), jsonSerializerSettings),
                        DateEnd = !dateEnd.HasValue ? "-" : dateEnd.Value.ToString(dateFormat),
                        DateBegin = !dateBegin.HasValue ? "-" : dateBegin.Value.ToString(dateFormat),
                        Status = GetRuleStatus(dateBegin, dateEnd).ToString(),
                        Conditions = string.IsNullOrWhiteSpace(conditions) ? string.Empty : JsonConvert.SerializeObject(conditions, jsonSerializerSettings)
                    });
                }
            }

            return Ok(list);
        }

        private static RuleStatus GetRuleStatus(DateTime? dateBegin, DateTime? dateEnd)
        {
            if (!dateBegin.HasValue)
            {
                return RuleStatus.Inactive;
            }

            if (dateBegin.Value > DateTime.UtcNow)
            {
                return RuleStatus.Pending;
            }

            if (!dateEnd.HasValue)
            {
                return RuleStatus.Active;
            }

            if (dateEnd.Value <= DateTime.UtcNow)
            {
                return RuleStatus.Inactive;
            }

            return RuleStatus.Active;
        }
    }
}