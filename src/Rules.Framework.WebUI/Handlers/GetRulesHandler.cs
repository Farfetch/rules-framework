namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.AspNetCore.Http;
    using Rules.Framework.Generics;
    using Rules.Framework.WebUI.Dto;
    using Rules.Framework.WebUI.Extensions;

    internal sealed class GetRulesHandler : WebUIRequestHandlerBase
    {
        private static readonly string[] resourcePath = new[] { "/{0}/api/v1/rules" };
        private readonly IGenericRulesEngine genericRulesEngine;
        private readonly IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer;

        public GetRulesHandler(IGenericRulesEngine genericRulesEngine, IRuleStatusDtoAnalyzer ruleStatusDtoAnalyzer, WebUIOptions webUIOptions) : base(resourcePath, webUIOptions)
        {
            this.genericRulesEngine = genericRulesEngine;
            this.ruleStatusDtoAnalyzer = ruleStatusDtoAnalyzer;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override async Task HandleRequestAsync(HttpRequest httpRequest,
            HttpResponse httpResponse,
            RequestDelegate next)
        {
            var rulesFilter = this.GetRulesFilterFromRequest(httpRequest);

            if (!IsValidFilterDates(rulesFilter))
            {
                await this.WriteResponseAsync(httpResponse, new { Message = "Date begin cannot be greater than after" }, (int)HttpStatusCode.BadRequest)
                   .ConfigureAwait(false);

                return;
            }

            try
            {
                var rules = new List<RuleDto>();

                if (rulesFilter.ContentType.Equals("all"))
                {
                    var contents = this.genericRulesEngine.GetContentTypes();

                    foreach (var identifier in contents.Select(c => c.Identifier))
                    {
                        var rulesForContentType = await this.GetRulesForContentyType(identifier, rulesFilter).ConfigureAwait(false);
                        rules.AddRange(rulesForContentType);
                    }
                }
                else
                {
                    var rulesForContentType = await this.GetRulesForContentyType(rulesFilter.ContentType, rulesFilter).ConfigureAwait(false);
                    rules.AddRange(rulesForContentType);
                }

                await this.WriteResponseAsync(httpResponse, rules, (int)HttpStatusCode.OK).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await this.WriteExceptionResponseAsync(httpResponse, ex).ConfigureAwait(false);
            }
        }

        private static bool IsValidFilterDates(RulesFilterDto rulesFilter)
        {
            return (rulesFilter.DateBegin is null
                || rulesFilter.DateEnd is null) ||
                (rulesFilter.DateBegin <= rulesFilter.DateEnd);
        }

        private IEnumerable<RuleDto> ApplyFilters(RulesFilterDto rulesFilter, IEnumerable<RuleDto> genericRulesDto)
        {
            if (!string.IsNullOrWhiteSpace(rulesFilter.Content))
            {
                genericRulesDto = genericRulesDto.Where(g =>
                {
                    return JsonSerializer.Serialize(g.Value).Contains(rulesFilter.Content, StringComparison.OrdinalIgnoreCase);
                });
            }

            if (!string.IsNullOrWhiteSpace(rulesFilter.Name))
            {
                genericRulesDto = genericRulesDto.Where(g =>
                {
                    return g.Name.Contains(rulesFilter.Name, StringComparison.OrdinalIgnoreCase);
                });
            }
            if (rulesFilter.Status != null)
            {
                genericRulesDto = genericRulesDto.Where(g =>
                {
                    return g.Status.Equals(rulesFilter.Status.ToString());
                });
            }

            return genericRulesDto;
        }

        private RulesFilterDto GetRulesFilterFromRequest(HttpRequest httpRequest)
        {
            var parseQueryString = HttpUtility.ParseQueryString(httpRequest.QueryString.Value);

            var rulesFilterAsString = JsonSerializer.Serialize(parseQueryString.Cast<string>().ToDictionary(k => k, v => string.IsNullOrWhiteSpace(parseQueryString[v]) ? null : parseQueryString[v]));
            var rulesFilter = JsonSerializer.Deserialize<RulesFilterDto>(rulesFilterAsString, this.SerializerOptions);

            rulesFilter.ContentType = string.IsNullOrWhiteSpace(rulesFilter.ContentType) ? "all" : rulesFilter.ContentType;

            rulesFilter.DateEnd ??= DateTime.MaxValue;

            rulesFilter.DateBegin ??= DateTime.MinValue;

            return rulesFilter;
        }

        private async Task<IEnumerable<RuleDto>> GetRulesForContentyType(string identifier, RulesFilterDto rulesFilter)
        {
            var genericRules = await this.genericRulesEngine.SearchAsync(
                                       new SearchArgs<GenericContentType, GenericConditionType>(
                                           new GenericContentType { Identifier = identifier },
                                           rulesFilter.DateBegin.Value, rulesFilter.DateEnd.Value))
                                       .ConfigureAwait(false);

            var priorityCriteria = this.genericRulesEngine.GetPriorityCriteria();

            if (genericRules != null && genericRules.Any())
            {
                if (priorityCriteria == PriorityCriterias.BottommostRuleWins)
                {
                    genericRules = genericRules.OrderByDescending(r => r.Priority);
                }
                else
                {
                    genericRules = genericRules.OrderBy(r => r.Priority);
                }

                var genericRulesDto = this.ApplyFilters(rulesFilter, genericRules.Select(g => g.ToRuleDto(identifier, this.ruleStatusDtoAnalyzer)));

                return genericRulesDto;
            }

            return Enumerable.Empty<RuleDto>();
        }
    }
}
