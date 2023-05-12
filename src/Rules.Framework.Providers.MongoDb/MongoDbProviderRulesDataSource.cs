namespace Rules.Framework.Providers.MongoDb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Rules.Framework.Core;
    using Rules.Framework.Providers.MongoDb.DataModel;

    /// <summary>
    /// The rules data source implementation for usage backed with a Mongo DB database.
    /// </summary>
    /// <typeparam name="TContentType">The type of the content type.</typeparam>
    /// <typeparam name="TConditionType">The type of the condition type.</typeparam>
    /// <seealso cref="Rules.Framework.IRulesDataSource{TContentType, TConditionType}"/>
    public class MongoDbProviderRulesDataSource<TContentType, TConditionType> : IRulesDataSource<TContentType, TConditionType>
    {
        private readonly IMongoDatabase mongoDatabase;
        private readonly MongoDbProviderSettings mongoDbProviderSettings;
        private readonly IRuleFactory<TContentType, TConditionType> ruleFactory;

        internal MongoDbProviderRulesDataSource(
            IMongoClient mongoClient,
            MongoDbProviderSettings mongoDbProviderSettings,
            IRuleFactory<TContentType, TConditionType> ruleFactory)
        {
            if (mongoClient is null)
            {
                throw new ArgumentNullException(nameof(mongoClient));
            }

            this.mongoDbProviderSettings = mongoDbProviderSettings ?? throw new ArgumentNullException(nameof(mongoDbProviderSettings));
            this.ruleFactory = ruleFactory ?? throw new ArgumentNullException(nameof(ruleFactory));
            this.mongoDatabase = mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);
        }

        /// <summary>
        /// Adds a new rule to data source.
        /// </summary>
        /// <param name="rule">The rule.</param>
        public async Task AddRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            var rulesCollection = this.mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            var ruleDataModel = this.ruleFactory.CreateRule(rule);

            await rulesCollection.InsertOneAsync(ruleDataModel).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the rules categorized with specified <paramref name="contentType"/> between
        /// <paramref name="dateBegin"/> and <paramref name="dateEnd"/>.
        /// </summary>
        /// <param name="contentType">the content type categorization.</param>
        /// <param name="dateBegin">the filtering begin date.</param>
        /// <param name="dateEnd">the filtering end date.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesAsync(TContentType contentType, DateTime dateBegin, DateTime dateEnd)
        {
            var getRulesByContentTypeAndDatesInterval = MongoDbProviderRulesDataSource<TContentType, TConditionType>
                .BuildFilterByContentTypeAndDatesInterval(contentType, dateBegin, dateEnd);

            return await this.GetRulesAsync(getRulesByContentTypeAndDatesInterval).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the rules filtered by specified arguments.
        /// </summary>
        /// <param name="rulesFilterArgs">The rules filter arguments.</param>
        /// <returns></returns>
        public Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesByAsync(RulesFilterArgs<TContentType> rulesFilterArgs)
        {
            if (rulesFilterArgs is null)
            {
                throw new ArgumentNullException(nameof(rulesFilterArgs));
            }

            var filterDefinition = MongoDbProviderRulesDataSource<TContentType, TConditionType>
                .BuildFilterFromRulesFilterArgs(rulesFilterArgs);

            return this.GetRulesAsync(filterDefinition);
        }

        /// <summary>
        /// Updates the existent rule on data source.
        /// </summary>
        /// <param name="rule">The rule.</param>
        public async Task UpdateRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            var rulesCollection = this.mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            var ruleDataModel = this.ruleFactory.CreateRule(rule);

            var filterDefinition = Builders<RuleDataModel>.Filter.Eq(x => x.Name, ruleDataModel.Name);
            FieldDefinition<RuleDataModel, object> contentField = "Content";
            var updateDefinitions = new UpdateDefinition<RuleDataModel>[]
            {
                Builders<RuleDataModel>.Update.Set(contentField, (object)ruleDataModel.Content),
                Builders<RuleDataModel>.Update.Set(r => r.ContentType, ruleDataModel.ContentType),
                Builders<RuleDataModel>.Update.Set(r => r.DateBegin, ruleDataModel.DateBegin),
                Builders<RuleDataModel>.Update.Set(r => r.DateEnd, ruleDataModel.DateEnd),
                Builders<RuleDataModel>.Update.Set(r => r.Name, ruleDataModel.Name),
                Builders<RuleDataModel>.Update.Set(r => r.Priority, ruleDataModel.Priority),
                Builders<RuleDataModel>.Update.Set(r => r.Active, ruleDataModel.Active),
                Builders<RuleDataModel>.Update.Set(r => r.RootCondition, ruleDataModel.RootCondition),
            };

            var updateDefinition = Builders<RuleDataModel>.Update.Combine(updateDefinitions);

            await rulesCollection.UpdateOneAsync(filterDefinition, updateDefinition).ConfigureAwait(false);
        }

        private static FilterDefinition<RuleDataModel> BuildFilterByContentTypeAndDatesInterval(TContentType contentType, DateTime dateBegin, DateTime dateEnd)
        {
            var contentTypeFilter = Builders<RuleDataModel>.Filter.Eq(x => x.ContentType, contentType.ToString());

            var datesFilter = Builders<RuleDataModel>.Filter.And(
                Builders<RuleDataModel>.Filter.Lte(rule => rule.DateBegin, dateEnd),
                Builders<RuleDataModel>.Filter.Or(
                    Builders<RuleDataModel>.Filter.Gt(rule => rule.DateEnd, dateBegin),
                    Builders<RuleDataModel>.Filter.Eq(rule => rule.DateEnd, null))
                );

            return Builders<RuleDataModel>.Filter.And(contentTypeFilter, datesFilter);
        }

        private static FilterDefinition<RuleDataModel> BuildFilterFromRulesFilterArgs(RulesFilterArgs<TContentType> rulesFilterArgs)
        {
            var filtersToApply = new List<FilterDefinition<RuleDataModel>>(3);

            if (!object.Equals(rulesFilterArgs.ContentType, default(TContentType)))
            {
                filtersToApply.Add(Builders<RuleDataModel>.Filter.Eq(x => x.ContentType, rulesFilterArgs.ContentType.ToString()));
            }

            if (!string.IsNullOrWhiteSpace(rulesFilterArgs.Name))
            {
                filtersToApply.Add(Builders<RuleDataModel>.Filter.Eq(x => x.Name, rulesFilterArgs.Name));
            }

            if (rulesFilterArgs.Priority.HasValue)
            {
                filtersToApply.Add(Builders<RuleDataModel>.Filter.Eq(x => x.Priority, rulesFilterArgs.Priority.GetValueOrDefault()));
            }

            return filtersToApply.Any() ? Builders<RuleDataModel>.Filter.And(filtersToApply) : Builders<RuleDataModel>.Filter.Empty;
        }

        private async Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesAsync(FilterDefinition<RuleDataModel> getRulesByContentTypeAndDatesInterval)
        {
            var rulesCollection = this.mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            var fetchedRulesCursor = await rulesCollection.FindAsync(getRulesByContentTypeAndDatesInterval).ConfigureAwait(false);

            var fetchedRules = await fetchedRulesCursor.ToListAsync().ConfigureAwait(false);

            return fetchedRules.Select(r => this.ruleFactory.CreateRule(r));
        }
    }
}
