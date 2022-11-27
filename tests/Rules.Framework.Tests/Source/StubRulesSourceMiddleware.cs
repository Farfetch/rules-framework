namespace Rules.Framework.Tests.Source
{
    using Rules.Framework.Core;
    using Rules.Framework.Source;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class StubRulesSourceMiddleware<TContentType, TConditionType> : IRulesSourceMiddleware<TContentType, TConditionType>
    {
        private readonly List<string> middlewareMessages;

        public int AddRuleCalls { get; private set; }
        public int GetRulesCalls { get; private set; }
        public int GetRulesFilteredCalls { get; private set; }
        public int UpdateRulesCalls { get; private set; }
        public string Name { get; }

        public StubRulesSourceMiddleware(string name, List<string> middlewareMessages)
        {
            this.Name = name;
            this.middlewareMessages = middlewareMessages;
        }

        public async Task HandleAddRuleAsync(
            AddRuleArgs<TContentType, TConditionType> args,
            AddRuleDelegate<TContentType, TConditionType> next)
        {
            this.AddRuleCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> HandleGetRulesAsync(
            GetRulesArgs<TContentType> args,
            GetRulesDelegate<TContentType, TConditionType> next)
        {
            this.GetRulesCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            var rules = await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
            return rules;
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> HandleGetRulesFilteredAsync(
            GetRulesFilteredArgs<TContentType> args,
            GetRulesFilteredDelegate<TContentType, TConditionType> next)
        {
            this.GetRulesFilteredCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            var rules = await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
            return rules;
        }

        public async Task HandleUpdateRuleAsync(
            UpdateRuleArgs<TContentType, TConditionType> args,
            UpdateRuleDelegate<TContentType, TConditionType> next)
        {
            this.UpdateRulesCalls++;
            this.middlewareMessages.Add($"Enter {this.Name}.");
            await next.Invoke(args).ConfigureAwait(false);
            this.middlewareMessages.Add($"Exit {this.Name}.");
        }
    }
}
