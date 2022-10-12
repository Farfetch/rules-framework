namespace Rules.Framework.Admin.WebApi.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.VisualBasic;
    using Newtonsoft.Json;
    using Rules.Framework.Admin.WebApi.Response;

    public class RuleController : Controller
    {
        private readonly JsonSerializerSettings jsonSerializerSettings;
        private readonly IRulesService rulesService;

        public RuleController(IRulesService rulesService)
        {
            this.rulesService = rulesService;

            this.jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        }

        public static object GetProperty(object target, string name, CallType method = CallType.Get)
        {
            return Microsoft.VisualBasic.CompilerServices.Versioned.CallByName(target, name, method);
        }

        [Route("rules/{controller}/{conditionType}/list")]
        public async Task<IActionResult> List([FromRoute] string conditionType)
        {
            var list = new List<RuleDto>();

            var rules = await this.rulesService
                .FindRulesAsync(conditionType, DateTime.UtcNow);

            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    var priority = GetProperty(rule, "Priority");
                    var name = GetProperty(rule, "Name");
                    var contentContainer = GetProperty(rule, "ContentContainer");
                    var dateEnd = GetProperty(rule, "DateEnd") as DateTime?;
                    var dateBegin = GetProperty(rule, "DateBegin") as DateTime?;
                    var conditions = GetProperty(rule, "RootCondition");

                    list.Add(new RuleDto
                    {
                        Id = priority,
                        Name = name,
                        Value = (contentContainer as dynamic).GetContentAs<string>(),
                        DateEnd = !dateEnd.HasValue ? "-" : dateEnd.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                        DateStart = !dateBegin.HasValue ? "-" : dateBegin.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                        Active = true,
                        Conditions = conditions is null ? string.Empty : JsonConvert.SerializeObject(conditions, jsonSerializerSettings)
                    });
                }
            }

            return Ok(list);
        }
    }
}