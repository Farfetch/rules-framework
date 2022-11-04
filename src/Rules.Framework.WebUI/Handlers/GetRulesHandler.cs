namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Rules.Framework.WebUI.Dto;

    internal class GetRulesHandler : WebUIRequestHandlerBase
    {
        private const string dateFormat = "dd/MM/yyyy HH:mm:ss";
        private static readonly string[] resourcePath = new[] { "/rules/Rule/List" };
        private readonly IRulesEngine rulesEngine;

        public GetRulesHandler(IRulesEngine rulesEngine) : base(resourcePath)
        {
            this.rulesEngine = rulesEngine;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override async Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            if (!httpRequest.Query.TryGetValue("contentType", out var contentType))
            {
                await this.WriteResponseAsync(httpResponse, new { }, (int)HttpStatusCode.BadRequest);
                return;
            }

            try
            {
                var list = new List<RuleDto>();

                var rules = await this.rulesEngine
                    .SearchAsync(new SearchArgs<ContentType, ConditionType>(new ContentType
                    {
                        Name = contentType
                    }, DateTime.MinValue, DateTime.MaxValue));

                var priorityOption = this.rulesEngine
                   .GetPriorityCriterias();

                if (rules != null)
                {
                    if (priorityOption == PriorityCriterias.BottomMostRuleWins)
                    {
                        rules = rules.OrderByDescending(r => r.Priority);
                    }
                    else
                    {
                        rules = rules.OrderBy(r => r.Priority);
                    }

                    foreach (var rule in rules)
                    {
                        list.Add(new RuleDto
                        {
                            Priority = rule.Priority,
                            Name = rule.Name,
                            Value = JsonConvert.SerializeObject(rule.ContentContainer, this.jsonSerializerSettings).Replace("\"", "\'"),
                            DateEnd = !rule.DateEnd.HasValue ? "-" : rule.DateEnd.Value.ToString(dateFormat),
                            DateBegin = rule.DateBegin.ToString(dateFormat),
                            Status = GetRuleStatus(rule.DateBegin, rule.DateEnd).ToString(),
                            Conditions = rule.RootCondition is null ? string.Empty : JsonConvert.SerializeObject(rule.RootCondition, this.jsonSerializerSettings).Replace("\"", "\'")
                        });
                    }
                }

                await this.WriteResponseAsync(httpResponse, list, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                await this.WriteResponseAsync(httpResponse, ex.Message.ToString() + Environment.NewLine + ex.InnerException.ToString(), (int)HttpStatusCode.InternalServerError);
            }
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