
namespace Rules.Framework.Providers.MongoDb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Rules.Framework.Core;
    using Rules.Framework.Providers.MongoDb.DataModel;

    public class MongoDbProviderRulesDataSource<TContentType, TConditionType> : IRulesDataSource<TContentType, TConditionType>
    {
        private readonly IMongoClient mongoClient;
        private readonly MongoDbProviderSettings mongoDbProviderSettings;
        private readonly IRuleFactory<TContentType, TConditionType> ruleFactory;
        private readonly IMongoDatabase mongoDatabase;

        internal MongoDbProviderRulesDataSource(
            IMongoClient mongoClient,
            MongoDbProviderSettings mongoDbProviderSettings,
            IRuleFactory<TContentType, TConditionType> ruleFactory)
        {
            this.mongoDbProviderSettings = mongoDbProviderSettings ?? throw new ArgumentNullException(nameof(mongoDbProviderSettings));
            this.ruleFactory = ruleFactory ?? throw new ArgumentNullException(nameof(ruleFactory));
            this.mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
            this.mongoDatabase = this.mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);
        }

        public async Task<IEnumerable<Rule<TContentType, TConditionType>>> GetRulesAsync(TContentType contentType, DateTime dateBegin, DateTime dateEnd)
        {
            IMongoCollection<RuleDataModel> rulesCollection = this.mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);

            FilterDefinition<RuleDataModel> getRulesByContentTypeAndDatesInterval = MongoDbProviderRulesDataSource<TContentType, TConditionType>
                .BuildFilterByContentTypeAndDatesInterval(contentType, dateBegin, dateEnd);

            IAsyncCursor<RuleDataModel> fetchedRulesCursor = await rulesCollection.FindAsync(getRulesByContentTypeAndDatesInterval).ConfigureAwait(false);

            List<RuleDataModel> fetchedRules = await fetchedRulesCursor.ToListAsync().ConfigureAwait(false);

            return fetchedRules.Select(r => this.ruleFactory.CreateRule(r));
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
    }
}
