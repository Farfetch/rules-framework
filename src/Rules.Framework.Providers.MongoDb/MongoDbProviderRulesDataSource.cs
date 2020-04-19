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
    /// <seealso cref="Rules.Framework.IRulesDataSource{TContentType, TConditionType}" />
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
            IMongoCollection<RuleDataModel> rulesCollection = this.mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            RuleDataModel ruleDataModel = this.ruleFactory.CreateRule(rule);

            await rulesCollection.InsertOneAsync(ruleDataModel).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the rules categorized with specified <paramref name="contentType" /> between <paramref name="dateBegin" /> and <paramref name="dateEnd" />.
        /// </summary>
        /// <param name="contentType">the content type categorization.</param>
        /// <param name="dateBegin">the filtering begin date.</param>
        /// <param name="dateEnd">the filtering end date.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesAsync(TContentType contentType, DateTime dateBegin, DateTime dateEnd)
        {
            FilterDefinition<RuleDataModel> getRulesByContentTypeAndDatesInterval = MongoDbProviderRulesDataSource<TContentType, TConditionType>
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

            FilterDefinition<RuleDataModel> filterDefinition = MongoDbProviderRulesDataSource<TContentType, TConditionType>
                .BuildFilterFromRulesFilterArgs(rulesFilterArgs);

            return this.GetRulesAsync(filterDefinition);
        }

        /// <summary>
        /// Updates the existent rule on data source.
        /// </summary>
        /// <param name="rule">The rule.</param>
        public async Task UpdateRuleAsync(Rule<TContentType, TConditionType> rule)
        {
            IMongoCollection<RuleDataModel> rulesCollection = this.mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            RuleDataModel ruleDataModel = this.ruleFactory.CreateRule(rule);

            FilterDefinition<RuleDataModel> filterDefinition = Builders<RuleDataModel>.Filter.Eq(x => x.Name, ruleDataModel.Name);
            FieldDefinition<RuleDataModel, object> contentField = "Content";
            IEnumerable<UpdateDefinition<RuleDataModel>> updateDefinitions = new UpdateDefinition<RuleDataModel>[]
            {
                Builders<RuleDataModel>.Update.Set(contentField, (object)ruleDataModel.Content),
                Builders<RuleDataModel>.Update.Set(r => r.ContentType, ruleDataModel.ContentType),
                Builders<RuleDataModel>.Update.Set(r => r.DateBegin, ruleDataModel.DateBegin),
                Builders<RuleDataModel>.Update.Set(r => r.DateEnd, ruleDataModel.DateEnd),
                Builders<RuleDataModel>.Update.Set(r => r.Name, ruleDataModel.Name),
                Builders<RuleDataModel>.Update.Set(r => r.Priority, ruleDataModel.Priority),
                Builders<RuleDataModel>.Update.Set(r => r.RootCondition, ruleDataModel.RootCondition),
            };

            UpdateDefinition<RuleDataModel> updateDefinition = Builders<RuleDataModel>.Update.Combine(updateDefinitions);

            await rulesCollection.UpdateOneAsync(filterDefinition, updateDefinition).ConfigureAwait(false);
        }

        private static FilterDefinition<RuleDataModel> BuildFilterByContentTypeAndDatesInterval(TContentType contentType, DateTime dateBegin, DateTime dateEnd)
        {
            FilterDefinition<RuleDataModel> contentTypeFilter = Builders<RuleDataModel>.Filter.Eq(x => x.ContentType, contentType.ToString());

            // To fetch rules that begin during filtered interval but end after it.
            FilterDefinition<RuleDataModel> dateBeginFilter = Builders<RuleDataModel>.Filter.And(
                Builders<RuleDataModel>.Filter.Gte(x => x.DateBegin, dateBegin),
                Builders<RuleDataModel>.Filter.Lt(x => x.DateBegin, dateEnd));

            // To fetch rules that begun before filtered interval but end during it.
            FilterDefinition<RuleDataModel> dateEndFilter = Builders<RuleDataModel>.Filter.And(
                Builders<RuleDataModel>.Filter.Ne(x => x.DateEnd, null),
                Builders<RuleDataModel>.Filter.Gte(x => x.DateEnd, dateBegin),
                Builders<RuleDataModel>.Filter.Lt(x => x.DateEnd, dateEnd));

            // To fetch rules that begun before and end after filtered interval.
            FilterDefinition<RuleDataModel> slicingFilter = Builders<RuleDataModel>.Filter.And(
                Builders<RuleDataModel>.Filter.Lt(x => x.DateBegin, dateBegin),
                Builders<RuleDataModel>.Filter.Or(
                    Builders<RuleDataModel>.Filter.Eq(x => x.DateEnd, null),
                    Builders<RuleDataModel>.Filter.Gt(x => x.DateEnd, dateEnd)));

            return Builders<RuleDataModel>.Filter.And(contentTypeFilter, Builders<RuleDataModel>.Filter.Or(dateBeginFilter, dateEndFilter, slicingFilter));
        }

        private static FilterDefinition<RuleDataModel> BuildFilterFromRulesFilterArgs(RulesFilterArgs<TContentType> rulesFilterArgs)
        {
            List<FilterDefinition<RuleDataModel>> filtersToApply = new List<FilterDefinition<RuleDataModel>>(3);

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
            IMongoCollection<RuleDataModel> rulesCollection = this.mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            IAsyncCursor<RuleDataModel> fetchedRulesCursor = await rulesCollection.FindAsync(getRulesByContentTypeAndDatesInterval).ConfigureAwait(false);

            List<RuleDataModel> fetchedRules = await fetchedRulesCursor.ToListAsync().ConfigureAwait(false);

            return fetchedRules.Select(r => this.ruleFactory.CreateRule(r));
        }
    }
}