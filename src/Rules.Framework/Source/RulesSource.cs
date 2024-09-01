namespace Rules.Framework.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal sealed class RulesSource : IRulesSource
    {
        private readonly AddRuleDelegate addRuleDelegate;
        private readonly CreateContentTypeDelegate createContentTypeDelegate;
        private readonly GetContentTypesDelegate getContentTypesDelegate;
        private readonly GetRulesDelegate getRulesDelegate;
        private readonly GetRulesFilteredDelegate getRulesFilteredDelegate;
        private readonly IRulesDataSource rulesDataSource;
        private readonly UpdateRuleDelegate updateRuleDelegate;

        public RulesSource(
            IRulesDataSource rulesDataSource,
            IEnumerable<IRulesSourceMiddleware> middlewares)
        {
            var middlewaresLinkedList = new LinkedList<IRulesSourceMiddleware>(middlewares);
            this.addRuleDelegate = CreateAddRulePipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.createContentTypeDelegate = CreateCreateContentTypePipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.getContentTypesDelegate = CreateGetContentTypesPipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.getRulesDelegate = CreateGetRulesPipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.getRulesFilteredDelegate = CreateGetRulesFilteredPipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.updateRuleDelegate = CreateUpdateRulePipelineDelegate(rulesDataSource, middlewaresLinkedList);
            this.rulesDataSource = rulesDataSource;
        }

        public async Task AddRuleAsync(AddRuleArgs args)
        {
            await this.addRuleDelegate.Invoke(args).ConfigureAwait(false);
        }

        public async Task CreateContentTypeAsync(CreateContentTypeArgs args)
        {
            await this.createContentTypeDelegate(args).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> GetContentTypesAsync(GetContentTypesArgs args)
        {
            return await this.getContentTypesDelegate(args).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Rule>> GetRulesAsync(GetRulesArgs args)
        {
            return await this.getRulesDelegate.Invoke(args).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Rule>> GetRulesFilteredAsync(GetRulesFilteredArgs args)
        {
            return await this.getRulesFilteredDelegate.Invoke(args).ConfigureAwait(false);
        }

        public async Task UpdateRuleAsync(UpdateRuleArgs args)
        {
            await this.updateRuleDelegate.Invoke(args).ConfigureAwait(false);
        }

        private static AddRuleDelegate CreateAddRulePipelineDelegate(
            IRulesDataSource rulesDataSource,
            LinkedList<IRulesSourceMiddleware> middlewares)
        {
            AddRuleDelegate action = async (args) => await rulesDataSource.AddRuleAsync(args.Rule).ConfigureAwait(false);

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

        private static CreateContentTypeDelegate CreateCreateContentTypePipelineDelegate(
            IRulesDataSource rulesDataSource,
            LinkedList<IRulesSourceMiddleware> middlewares)
        {
            CreateContentTypeDelegate action = async (args) => await rulesDataSource.CreateContentTypeAsync(args.Name).ConfigureAwait(false);

            if (middlewares.Count > 0)
            {
                var middlewareNode = middlewares.Last;

                while (middlewareNode is { })
                {
                    var middleware = middlewareNode.Value;
                    var immutableAction = action;
                    action = async (args) => await middleware.HandleCreateContentTypeAsync(args, immutableAction).ConfigureAwait(false);

                    // Get previous middleware node.
                    middlewareNode = middlewareNode.Previous;
                }
            }

            return action;
        }

        private static GetContentTypesDelegate CreateGetContentTypesPipelineDelegate(
            IRulesDataSource rulesDataSource,
            LinkedList<IRulesSourceMiddleware> middlewares)
        {
            GetContentTypesDelegate action = async (_) => await rulesDataSource.GetContentTypesAsync().ConfigureAwait(false);

            if (middlewares.Count > 0)
            {
                var middlewareNode = middlewares.Last;

                while (middlewareNode is { })
                {
                    var middleware = middlewareNode.Value;
                    var immutableAction = action;
                    action = async (args) => await middleware.HandleGetContentTypesAsync(args, immutableAction).ConfigureAwait(false);

                    // Get previous middleware node.
                    middlewareNode = middlewareNode.Previous;
                }
            }

            return action;
        }

        private static GetRulesFilteredDelegate CreateGetRulesFilteredPipelineDelegate(
            IRulesDataSource rulesDataSource,
            LinkedList<IRulesSourceMiddleware> middlewares)
        {
            GetRulesFilteredDelegate action =
                async (args) =>
                {
                    RulesFilterArgs rulesFilterArgs = new()
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

        private static GetRulesDelegate CreateGetRulesPipelineDelegate(
            IRulesDataSource rulesDataSource,
            LinkedList<IRulesSourceMiddleware> middlewares)
        {
            GetRulesDelegate action =
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

        private static UpdateRuleDelegate CreateUpdateRulePipelineDelegate(
            IRulesDataSource rulesDataSource,
            LinkedList<IRulesSourceMiddleware> middlewares)
        {
            UpdateRuleDelegate action =
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