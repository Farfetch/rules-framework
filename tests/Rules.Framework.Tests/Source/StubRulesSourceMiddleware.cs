namespace Rules.Framework.Tests.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Source;

    internal class StubRulesSourceMiddleware : IRulesSourceMiddleware
    {
        private readonly List<string> middlewareMessages;

        public StubRulesSourceMiddleware(string name, List<string> middlewareMessages)
        {
            this.Name = name;
            this.middlewareMessages = middlewareMessages;
        }

        public int AddRuleCalls { get; private set; }
        public int CreateContentTypeCalls { get; private set; }
        public int GetContentTypesCalls { get; private set; }
        public int GetRulesCalls { get; private set; }
        public int GetRulesFilteredCalls { get; private set; }
        public string Name { get; }
        public int UpdateRulesCalls { get; private set; }

        public async Task HandleAddRuleAsync(
            AddRuleArgs args,
            AddRuleDelegate next)
        {
            this.AddRuleCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
        }

        public async Task HandleCreateContentTypeAsync(
            CreateContentTypeArgs args,
            CreateContentTypeDelegate next)
        {
            this.CreateContentTypeCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
        }

        public async Task<IEnumerable<string>> HandleGetContentTypesAsync(GetContentTypesArgs args, GetContentTypesDelegate next)
        {
            this.GetContentTypesCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            var contentTypes = await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
            return contentTypes;
        }

        public async Task<IEnumerable<Rule>> HandleGetRulesAsync(
            GetRulesArgs args,
            GetRulesDelegate next)
        {
            this.GetRulesCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            var rules = await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
            return rules;
        }

        public async Task<IEnumerable<Rule>> HandleGetRulesFilteredAsync(
            GetRulesFilteredArgs args,
            GetRulesFilteredDelegate next)
        {
            this.GetRulesFilteredCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            var rules = await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
            return rules;
        }

        public async Task HandleUpdateRuleAsync(
            UpdateRuleArgs args,
            UpdateRuleDelegate next)
        {
            this.UpdateRulesCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
        }
    }
}