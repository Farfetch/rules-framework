namespace Rules.Framework.WebApi.Controllers
{
    using System;
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

        [Route("rules/{controller}/options")]
        public async Task<IActionResult> GetOptions()
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
                .SearchAsync(new SearchArgs<ContentType, ConditionType>(new ContentType { Name = conditionType }, DateTime.MinValue, DateTime.MaxValue));

            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    int? priority = GetProperty(rule, "Priority") as int?;
                    DateTime? dateEnd = GetProperty(rule, "DateEnd") as DateTime?;
                    DateTime? dateBegin = GetProperty(rule, "DateBegin") as DateTime?;
                    var name = GetProperty(rule, "Name") as string;
                    var contentContainer = GetProperty(rule, "ContentContainer") as dynamic;
                    var conditions = GetProperty(rule, "RootCondition");

                    list.Add(new RuleDto
                    {
                        Priority = !priority.HasValue ? 0 : priority.Value,
                        Name = name,
                        Value = contentContainer is null ? "" : JsonConvert.SerializeObject(contentContainer.GetContentAs<dynamic>(), jsonSerializerSettings),
                        DateEnd = !dateEnd.HasValue ? "-" : dateEnd.Value.ToString(dateFormat),
                        DateBegin = !dateBegin.HasValue ? "-" : dateBegin.Value.ToString(dateFormat),
                        Status = GetRuleStatus(dateBegin, dateEnd).ToString(),
                        Conditions = conditions is null ? string.Empty : JsonConvert.SerializeObject(conditions, jsonSerializerSettings)
                    });
                }
            }

            return Ok(list);
        }

        private static object GetProperty(object instance, string strPropertyName)
        {
            Type type = instance.GetType();
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(strPropertyName);
            return propertyInfo.GetValue(instance, null);
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