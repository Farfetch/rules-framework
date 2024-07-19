namespace Rules.Framework.WebUI.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    internal sealed class GetConfigurationsHandler : WebUIRequestHandlerBase
    {
        private static readonly string[] resourcePath = new[] { "/{0}/api/v1/configurations" };

        private readonly IRulesEngine rulesEngine;

        public GetConfigurationsHandler(IRulesEngine rulesEngine, WebUIOptions webUIOptions) : base(resourcePath, webUIOptions)
        {
            this.rulesEngine = rulesEngine;
        }

        protected override HttpMethod HttpMethod => HttpMethod.GET;

        protected override Task HandleRequestAsync(HttpRequest httpRequest, HttpResponse httpResponse, RequestDelegate next)
        {
            try
            {
                var configurations = new Dictionary<string, object>
                {
                    { "PriorityCriteria", this.rulesEngine.Options.PriorityCriteria.ToString() },
                    { "MissingConditionBehavior", this.rulesEngine.Options.MissingConditionBehavior.ToString() },
                    {
                        "DataTypeDefaults",
                        new Dictionary<string, string>
                        {
                            { "ArrayBoolean", this.rulesEngine.Options.DataTypeDefaults[DataTypes.ArrayBoolean].ToString() },
                            { "ArrayDecimal", this.rulesEngine.Options.DataTypeDefaults[DataTypes.ArrayDecimal].ToString() },
                            { "ArrayInteger", this.rulesEngine.Options.DataTypeDefaults[DataTypes.ArrayInteger].ToString() },
                            { "ArrayString", this.rulesEngine.Options.DataTypeDefaults[DataTypes.ArrayString].ToString() },
                            { "Boolean", this.rulesEngine.Options.DataTypeDefaults[DataTypes.Boolean].ToString() },
                            { "Decimal", this.rulesEngine.Options.DataTypeDefaults[DataTypes.Decimal].ToString() },
                            { "Integer", this.rulesEngine.Options.DataTypeDefaults[DataTypes.Integer].ToString() },
                            { "String", this.rulesEngine.Options.DataTypeDefaults[DataTypes.String].ToString() },
                        }
                    }
                };

                return this.WriteResponseAsync(httpResponse, configurations, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.WriteExceptionResponseAsync(httpResponse, ex);
            }
        }
    }
}