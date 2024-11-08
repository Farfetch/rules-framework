namespace Rules.Framework.Providers.MongoDb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Rules.Framework.Providers.MongoDb.DataModel;

    /// <summary>
    /// The rules data source implementation for usage backed with a Mongo DB database.
    /// </summary>
    /// <seealso cref="Rules.Framework.IRulesDataSource"/>
    public class MongoDbProviderRulesDataSource : IRulesDataSource
    {
        private readonly IMongoDatabase mongoDatabase;
        private readonly MongoDbProviderSettings mongoDbProviderSettings;
        private readonly IRuleFactory ruleFactory;

        internal MongoDbProviderRulesDataSource(
            IMongoClient mongoClient,
            MongoDbProviderSettings mongoDbProviderSettings,
            IRuleFactory ruleFactory)
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
        public async Task AddRuleAsync(Rule rule)
        {
            var rulesCollection = this.mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            var ruleDataModel = this.ruleFactory.CreateRule(rule);

            await rulesCollection.InsertOneAsync(ruleDataModel).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new ruleset on the data source.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        public async Task CreateRulesetAsync(string contentType)
        {
            var rulesetsCollection = this.mongoDatabase.GetCollection<RulesetDataModel>(this.mongoDbProviderSettings.RulesetsCollectionName);

            var rulesetDataModel = new RulesetDataModel
            {
                Creation = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Name = contentType,
            };

            await rulesetsCollection.InsertOneAsync(rulesetDataModel).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the rules categorized with specified <paramref name="ruleset"/> between <paramref
        /// name="dateBegin"/> and <paramref name="dateEnd"/>.
        /// </summary>
        /// <param name="ruleset">the ruleset name.</param>
        /// <param name="dateBegin">the filtering begin date.</param>
        /// <param name="dateEnd">the filtering end date.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Rule>> GetRulesAsync(string ruleset, DateTime dateBegin, DateTime dateEnd)
        {
            var getRulesByRulesetAndDatesInterval = MongoDbProviderRulesDataSource
                .BuildFilterByContentTypeAndDatesInterval(ruleset, dateBegin, dateEnd);

            return await this.GetRulesAsync(getRulesByRulesetAndDatesInterval).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the rules filtered by specified arguments.
        /// </summary>
        /// <param name="rulesFilterArgs">The rules filter arguments.</param>
        /// <returns></returns>
        public Task<IEnumerable<Rule>> GetRulesByAsync(RulesFilterArgs rulesFilterArgs)
        {
            if (rulesFilterArgs is null)
            {
                throw new ArgumentNullException(nameof(rulesFilterArgs));
            }

            var filterDefinition = MongoDbProviderRulesDataSource
                .BuildFilterFromRulesFilterArgs(rulesFilterArgs);

            return this.GetRulesAsync(filterDefinition);
        }

        /// <summary>
        /// Gets the content types from the data source.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Ruleset>> GetRulesetsAsync()
        {
            var rulesetsCollection = this.mongoDatabase.GetCollection<RulesetDataModel>(this.mongoDbProviderSettings.RulesetsCollectionName);

            var findAllFilterDefinition = FilterDefinition<RulesetDataModel>.Empty;

            var resultsCursor = await rulesetsCollection.FindAsync(findAllFilterDefinition).ConfigureAwait(false);
            var rulesets = new List<Ruleset>();
            while (await resultsCursor.MoveNextAsync().ConfigureAwait(false))
            {
                foreach (var rulesetDataModel in resultsCursor.Current)
                {
                    rulesets.Add(new Ruleset(rulesetDataModel.Name, rulesetDataModel.Creation));
                }
            }

            return rulesets;
        }

        /// <summary>
        /// Updates the existent rule on data source.
        /// </summary>
        /// <param name="rule">The rule.</param>
        public async Task UpdateRuleAsync(Rule rule)
        {
            var rulesCollection = this.mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            var ruleDataModel = this.ruleFactory.CreateRule(rule);

            var filterDefinition = Builders<RuleDataModel>.Filter.Eq(x => x.Name, ruleDataModel.Name);
            FieldDefinition<RuleDataModel, object> contentField = "Content";
            var updateDefinitions = new UpdateDefinition<RuleDataModel>[]
            {
                Builders<RuleDataModel>.Update.Set(contentField, (object)ruleDataModel.Content),
                Builders<RuleDataModel>.Update.Set(r => r.Ruleset, ruleDataModel.Ruleset),
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

        private static FilterDefinition<RuleDataModel> BuildFilterByContentTypeAndDatesInterval(string ruleset, DateTime dateBegin, DateTime dateEnd)
        {
            var rulesetFilter = Builders<RuleDataModel>.Filter.Eq(x => x.Ruleset, ruleset);

            var datesFilter = Builders<RuleDataModel>.Filter.And(
                Builders<RuleDataModel>.Filter.Lte(rule => rule.DateBegin, dateEnd),
                Builders<RuleDataModel>.Filter.Or(
                    Builders<RuleDataModel>.Filter.Gt(rule => rule.DateEnd, dateBegin),
                    Builders<RuleDataModel>.Filter.Eq(rule => rule.DateEnd, null))
                );

            return Builders<RuleDataModel>.Filter.And(rulesetFilter, datesFilter);
        }

        private static FilterDefinition<RuleDataModel> BuildFilterFromRulesFilterArgs(RulesFilterArgs rulesFilterArgs)
        {
            var filtersToApply = new List<FilterDefinition<RuleDataModel>>(3);

            if (!object.Equals(rulesFilterArgs.Ruleset, default(string)))
            {
                filtersToApply.Add(Builders<RuleDataModel>.Filter.Eq(x => x.Ruleset, rulesFilterArgs.Ruleset));
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

        private async Task<IEnumerable<Rule>> GetRulesAsync(FilterDefinition<RuleDataModel> getRulesByContentTypeAndDatesInterval)
        {
            var rulesCollection = this.mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            var fetchedRulesCursor = await rulesCollection.FindAsync(getRulesByContentTypeAndDatesInterval).ConfigureAwait(false);

            var fetchedRules = await fetchedRulesCursor.ToListAsync().ConfigureAwait(false);

            // We won't use LINQ from this point onwards to avoid projected queries to database at a
            // later point. This approach assures the definitive realization of the query results
            // and does not produce side effects later on.
            var result = new Rule[fetchedRules.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = this.ruleFactory.CreateRule(fetchedRules[i]);
            }

            return result;
        }
    }
}