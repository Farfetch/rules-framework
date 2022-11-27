namespace Rules.Framework.Source
{
    using Rules.Framework.Core;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class RulesSource<TContentType, TConditionType> : IRulesSource<TContentType, TConditionType>
    {
        private readonly List<IRulesSourceMiddleware<TContentType, TConditionType>> middlewares;
        private readonly IRulesDataSource<TContentType, TConditionType> rulesDataSource;

        public RulesSource(
            IRulesDataSource<TContentType, TConditionType> rulesDataSource,
            IEnumerable<IRulesSourceMiddleware<TContentType, TConditionType>> middlewares)
        {
            this.middlewares = new List<IRulesSourceMiddleware<TContentType, TConditionType>>(middlewares);
            this.rulesDataSource = rulesDataSource;
        }

        public async Task AddRuleAsync(AddRuleArgs<TContentType, TConditionType> args)
        {
            var queuedMiddlewares = new Queue<IRulesSourceMiddleware<TContentType, TConditionType>>(this.middlewares);

            await this.AddRuleInternalAsync(args, queuedMiddlewares).ConfigureAwait(false);
        }

        private async Task AddRuleInternalAsync(
            AddRuleArgs<TContentType, TConditionType> args,
            Queue<IRulesSourceMiddleware<TContentType, TConditionType>> queuedMiddlewares)
        {
            if (queuedMiddlewares.Count > 0)
            {
                IRulesSourceMiddleware<TContentType, TConditionType> middleware = queuedMiddlewares.Dequeue();
                if (queuedMiddlewares.Count > 0)
                {
                    await middleware.HandleAddRuleAsync(
                        args,
                        GetDelegateForNextMiddleware(queuedMiddlewares)).ConfigureAwait(false);
                    return;
                }

                await middleware.HandleAddRuleAsync(
                    args,
                    GetDelegateForDataSource()).ConfigureAwait(false);
                return;
            }

            await ExecuteDataSource(args).ConfigureAwait(false);

            AddRuleDelegate<TContentType, TConditionType> GetDelegateForNextMiddleware(Queue<IRulesSourceMiddleware<TContentType, TConditionType>> queuedMiddlewares)
                => async (argsInternal) => await this.AddRuleInternalAsync(argsInternal, queuedMiddlewares).ConfigureAwait(false);

            AddRuleDelegate<TContentType, TConditionType> GetDelegateForDataSource() => ExecuteDataSource;

            async Task ExecuteDataSource(AddRuleArgs<TContentType, TConditionType> args)
            {
                await this.rulesDataSource.AddRuleAsync(args.Rule).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesAsync(GetRulesArgs<TContentType> args)
        {
            var queuedMiddlewares = new Queue<IRulesSourceMiddleware<TContentType, TConditionType>>(this.middlewares);

            return await this.GetRulesInternalAsync(args, queuedMiddlewares).ConfigureAwait(false);
        }

        private async Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesInternalAsync(
            GetRulesArgs<TContentType> args,
            Queue<IRulesSourceMiddleware<TContentType, TConditionType>> queuedMiddlewares)
        {
            if (queuedMiddlewares.Count > 0)
            {
                IRulesSourceMiddleware<TContentType, TConditionType> middleware = queuedMiddlewares.Dequeue();
                if (queuedMiddlewares.Count > 0)
                {
                    return await middleware.HandleGetRulesAsync(
                        args,
                        GetDelegateForNextMiddleware(queuedMiddlewares)).ConfigureAwait(false);
                }

                return await middleware.HandleGetRulesAsync(
                    args,
                    GetDelegateForDataSource()).ConfigureAwait(false);
            }

            return await ExecuteDataSource(args).ConfigureAwait(false);

            GetRulesDelegate<TContentType, TConditionType> GetDelegateForNextMiddleware(Queue<IRulesSourceMiddleware<TContentType, TConditionType>> queuedMiddlewares)
                => async (argsInternal) => await this.GetRulesInternalAsync(argsInternal, queuedMiddlewares).ConfigureAwait(false);

            GetRulesDelegate<TContentType, TConditionType> GetDelegateForDataSource() => ExecuteDataSource;

            async Task<IEnumerable<Rule<TContentType, TConditionType>>> ExecuteDataSource(GetRulesArgs<TContentType> args)
                => await this.rulesDataSource.GetRulesAsync(args.ContentType, args.DateBegin, args.DateEnd).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesFilteredAsync(GetRulesFilteredArgs<TContentType> args)
        {
            var queuedMiddlewares = new Queue<IRulesSourceMiddleware<TContentType, TConditionType>>(this.middlewares);

            return await this.GetRulesFilteredInternalAsync(args, queuedMiddlewares).ConfigureAwait(false);
        }

        private async Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesFilteredInternalAsync(
            GetRulesFilteredArgs<TContentType> args,
            Queue<IRulesSourceMiddleware<TContentType, TConditionType>> queuedMiddlewares)
        {
            if (queuedMiddlewares.Count > 0)
            {
                IRulesSourceMiddleware<TContentType, TConditionType> middleware = queuedMiddlewares.Dequeue();
                if (queuedMiddlewares.Count > 0)
                {
                    return await middleware.HandleGetRulesFilteredAsync(
                        args,
                        GetDelegateForNextMiddleware(queuedMiddlewares)).ConfigureAwait(false);
                }

                return await middleware.HandleGetRulesFilteredAsync(
                    args,
                    GetDelegateForDataSource()).ConfigureAwait(false);
            }

            return await ExecuteDataSource(args).ConfigureAwait(false);

            GetRulesFilteredDelegate<TContentType, TConditionType> GetDelegateForNextMiddleware(Queue<IRulesSourceMiddleware<TContentType, TConditionType>> queuedMiddlewares)
                => async (argsInternal) => await this.GetRulesFilteredInternalAsync(argsInternal, queuedMiddlewares).ConfigureAwait(false);

            GetRulesFilteredDelegate<TContentType, TConditionType> GetDelegateForDataSource() => ExecuteDataSource;

            async Task<IEnumerable<Rule<TContentType, TConditionType>>> ExecuteDataSource(GetRulesFilteredArgs<TContentType> args)
                {
                    RulesFilterArgs<TContentType> rulesFilterArgs = new()
                    {
                        ContentType = args.ContentType,
                        Name = args.Name,
                        Priority = args.Priority,
                    };

                    return await this.rulesDataSource.GetRulesByAsync(rulesFilterArgs).ConfigureAwait(false);
                }
        }

        public async Task UpdateRuleAsync(UpdateRuleArgs<TContentType, TConditionType> args)
        {
            var queuedMiddlewares = new Queue<IRulesSourceMiddleware<TContentType, TConditionType>>(this.middlewares);

            await this.UpdateRuleInternalAsync(args, queuedMiddlewares).ConfigureAwait(false);
        }

        private async Task UpdateRuleInternalAsync(
            UpdateRuleArgs<TContentType, TConditionType> args,
            Queue<IRulesSourceMiddleware<TContentType, TConditionType>> queuedMiddlewares)
        {
            if (queuedMiddlewares.Count > 0)
            {
                IRulesSourceMiddleware<TContentType, TConditionType> middleware = queuedMiddlewares.Dequeue();
                if (queuedMiddlewares.Count > 0)
                {
                    await middleware.HandleUpdateRuleAsync(
                        args,
                        GetDelegateForNextMiddleware(queuedMiddlewares)).ConfigureAwait(false);
                    return;
                }

                await middleware.HandleUpdateRuleAsync(
                    args,
                    GetDelegateForDataSource()).ConfigureAwait(false);
                return;
            }

            await ExecuteDataSource(args).ConfigureAwait(false);

            UpdateRuleDelegate<TContentType, TConditionType> GetDelegateForNextMiddleware(Queue<IRulesSourceMiddleware<TContentType, TConditionType>> queuedMiddlewares)
                => async (argsInternal) => await this.UpdateRuleInternalAsync(argsInternal, queuedMiddlewares).ConfigureAwait(false);

            UpdateRuleDelegate<TContentType, TConditionType> GetDelegateForDataSource() => ExecuteDataSource;

            async Task ExecuteDataSource(UpdateRuleArgs<TContentType, TConditionType> args)
                => await this.rulesDataSource.UpdateRuleAsync(args.Rule).ConfigureAwait(false);
        }
    }
}
