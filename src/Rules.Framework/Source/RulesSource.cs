namespace Rules.Framework.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Rules.Framework.Core;

    internal sealed class RulesSource<TContentType, TConditionType> : IRulesSource<TContentType, TConditionType>
    {
        private readonly AddRuleDelegate<TContentType, TConditionType> addRuleDelegate;
        private readonly GetRulesDelegate<TContentType, TConditionType> getRulesDelegate;
        private readonly GetRulesFilteredDelegate<TContentType, TConditionType> getRulesFilteredDelegate;
        private readonly UpdateRuleDelegate<TContentType, TConditionType> updateRuleDelegate;

        public RulesSource(
            IRulesDataSource<TContentType, TConditionType> rulesDataSource,
            IEnumerable<IRulesSourceMiddleware<TContentType, TConditionType>> middlewares)
        {
            var middlewaresLinkedList = new LinkedList<IRulesSourceMiddleware<TContentType, TConditionType>>(middlewares);
            this.addRuleDelegate = CreateAddRulePipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.getRulesDelegate = CreateGetRulesPipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.getRulesFilteredDelegate = CreateGetRulesFilteredPipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.updateRuleDelegate = CreateUpdateRulePipelineDelegate(rulesDataSource, middlewaresLinkedList);
        }

        public async Task AddRuleAsync(AddRuleArgs<TContentType, TConditionType> args)
        {
            await this.addRuleDelegate.Invoke(args).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesAsync(GetRulesArgs<TContentType> args)
        {
            return await this.getRulesDelegate.Invoke(args).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesFilteredAsync(GetRulesFilteredArgs<TContentType> args)
        {
            return await this.getRulesFilteredDelegate.Invoke(args).ConfigureAwait(false);
        }

        public async Task UpdateRuleAsync(UpdateRuleArgs<TContentType, TConditionType> args)
        {
            await this.updateRuleDelegate.Invoke(args).ConfigureAwait(false);
        }

        private static AddRuleDelegate<TContentType, TConditionType> CreateAddRulePipelineDelegate(
            IRulesDataSource<TContentType, TConditionType> rulesDataSource,
            LinkedList<IRulesSourceMiddleware<TContentType, TConditionType>> middlewares)
        {
            AddRuleDelegate<TContentType, TConditionType> action = async (args) => await rulesDataSource.AddRuleAsync(args.Rule).ConfigureAwait(false);

            if (middlewares.Count > 0)
            {
                var middlewareNode = middlewares.Last;

                while (middlewareNode is { })
                {
                    var middleware = middlewareNode.Value;
                    var immutableAction = action;
                    action = async (args) => await middleware.HandleAddRuleAsync(args, immutableAction).ConfigureAwait(false);

                    // Get previous middleware node.
                    middlewareNode = middlewareNode.Previous;
                }
            }

            return action;
        }

        private static GetRulesFilteredDelegate<TContentType, TConditionType> CreateGetRulesFilteredPipelineDelegate(
            IRulesDataSource<TContentType, TConditionType> rulesDataSource,
            LinkedList<IRulesSourceMiddleware<TContentType, TConditionType>> middlewares)
        {
            GetRulesFilteredDelegate<TContentType, TConditionType> action =
                async (args) =>
                {
                    RulesFilterArgs<TContentType> rulesFilterArgs = new()
                    {
                        ContentType = args.ContentType,
                        Name = args.Name,
                        Priority = args.Priority,
                    };

                    return await rulesDataSource.GetRulesByAsync(rulesFilterArgs).ConfigureAwait(false);
                };

            if (middlewares.Count > 0)
            {
                var middlewareNode = middlewares.Last;

                while (middlewareNode is { })
                {
                    var middleware = middlewareNode.Value;
                    var immutableAction = action;
                    action = async (args) => await middleware.HandleGetRulesFilteredAsync(args, immutableAction).ConfigureAwait(false);

                    // Get previous middleware node.
                    middlewareNode = middlewareNode.Previous;
                }
            }

            return action;
        }

        private static GetRulesDelegate<TContentType, TConditionType> CreateGetRulesPipelineDelegate(
            IRulesDataSource<TContentType, TConditionType> rulesDataSource,
            LinkedList<IRulesSourceMiddleware<TContentType, TConditionType>> middlewares)
        {
            GetRulesDelegate<TContentType, TConditionType> action =
                async (args)
                    => await rulesDataSource.GetRulesAsync(args.ContentType, args.DateBegin, args.DateEnd).ConfigureAwait(false);

            if (middlewares.Count > 0)
            {
                var middlewareNode = middlewares.Last;

                while (middlewareNode is { })
                {
                    var middleware = middlewareNode.Value;
                    var immutableAction = action;
                    action = async (args) => await middleware.HandleGetRulesAsync(args, immutableAction).ConfigureAwait(false);

                    // Get previous middleware node.
                    middlewareNode = middlewareNode.Previous;
                }
            }

            return action;
        }

        private static UpdateRuleDelegate<TContentType, TConditionType> CreateUpdateRulePipelineDelegate(
            IRulesDataSource<TContentType, TConditionType> rulesDataSource,
            LinkedList<IRulesSourceMiddleware<TContentType, TConditionType>> middlewares)
        {
            UpdateRuleDelegate<TContentType, TConditionType> action =
                async (args)
                    => await rulesDataSource.UpdateRuleAsync(args.Rule).ConfigureAwait(false);

            if (middlewares.Count > 0)
            {
                var middlewareNode = middlewares.Last;

                while (middlewareNode is { })
                {
                    var middleware = middlewareNode.Value;
                    var immutableAction = action;
                    action = async (args) => await middleware.HandleUpdateRuleAsync(args, immutableAction).ConfigureAwait(false);

                    // Get previous middleware node.
                    middlewareNode = middlewareNode.Previous;
                }
            }

            return action;
        }
    }
}