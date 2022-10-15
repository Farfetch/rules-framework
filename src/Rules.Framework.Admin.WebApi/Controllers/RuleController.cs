namespace Rules.Framework.Admin.WebApi.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
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

        public static object GetProperty(object instance, string strPropertyName)
        {
            Type type = instance.GetType();
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(strPropertyName);
            return propertyInfo.GetValue(instance, null);
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
                    int? priority = GetProperty(rule, "Priority") as int?;
                    DateTime? dateEnd = GetProperty(rule, "DateEnd") as DateTime?;
                    DateTime? dateBegin = GetProperty(rule, "DateBegin") as DateTime?;
                    var name = GetProperty(rule, "Name") as string;
                    var contentContainer = GetProperty(rule, "ContentContainer");
                    var conditions = GetProperty(rule, "RootCondition") as string;

                    list.Add(new RuleDto
                    {
                        Priority = !priority.HasValue ? 0 : priority.Value,
                        Name = name,
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