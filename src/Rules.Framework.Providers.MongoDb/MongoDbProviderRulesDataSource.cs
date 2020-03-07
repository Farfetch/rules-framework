
namespace Rules.Framework.Providers.MongoDb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Rules.Framework.Core;
    using Rules.Framework.Core.ConditionNodes;
    using Rules.Framework.Providers.MongoDb.DataModel;
    using Rules.Framework.Serialization;

    public class MongoDbProviderRulesDataSource<TContentType, TConditionType> : IRulesDataSource<TContentType, TConditionType>
    {
        private IMongoClient mongoClient;
        private readonly MongoDbProviderSettings mongoDbProviderSettings;
        private readonly IContentSerializationProvider<TContentType> contentSerializationProvider;
        private IMongoDatabase mongoDatabase;

        public MongoDbProviderRulesDataSource(
            IMongoClient mongoClient,
            MongoDbProviderSettings mongoDbProviderSettings,
            IContentSerializationProvider<TContentType> contentSerializationProvider)
        {
            this.mongoDbProviderSettings = mongoDbProviderSettings ?? throw new ArgumentNullException(nameof(mongoDbProviderSettings));
            this.contentSerializationProvider = contentSerializationProvider ?? throw new ArgumentNullException(nameof(contentSerializationProvider));
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

            return fetchedRules.Select(r => this.CreateRule(r));
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

        private Rule<TContentType, TConditionType> CreateRule(RuleDataModel ruleDataModel)
        {
            IConditionNode<TConditionType> rootCondition = MongoDbProviderRulesDataSource<TContentType, TConditionType>.ConvertConditionNode(ruleDataModel.RootCondition);
            TContentType contentType = MongoDbProviderRulesDataSource<TContentType, TConditionType>.Parse<TContentType>(ruleDataModel.ContentType);
            ContentContainer<TContentType> contentContainer = new SerializedContentContainer<TContentType>(contentType, ruleDataModel.Content, this.contentSerializationProvider);

            return new Rule<TContentType, TConditionType>()
            {
                ContentContainer = contentContainer,
                DateBegin = ruleDataModel.DateBegin,
                DateEnd = ruleDataModel.DateEnd,
                Name = ruleDataModel.Name,
                Priority = ruleDataModel.Priority,
                RootCondition = rootCondition
            };
        }

        private static IConditionNode<TConditionType> ConvertConditionNode(ConditionNodeDataModel conditionNodeDataModel)
        {
            if (conditionNodeDataModel.LogicalOperator == LogicalOperators.Eval)
            {
                return MongoDbProviderRulesDataSource<TContentType, TConditionType>.CreateValueConditionNode(conditionNodeDataModel as ValueConditionNodeDataModel);
            }
            else
            {
                ComposedConditionNodeDataModel composedConditionNodeDataModel = conditionNodeDataModel as ComposedConditionNodeDataModel;

                List<IConditionNode<TConditionType>> childConditionNodes =
                    new List<IConditionNode<TConditionType>>(composedConditionNodeDataModel.ChildConditionNodes.Count());
                foreach (ConditionNodeDataModel child in composedConditionNodeDataModel.ChildConditionNodes)
                {
                    childConditionNodes.Add(MongoDbProviderRulesDataSource<TContentType, TConditionType>.ConvertConditionNode(child));
                }

                return new ComposedConditionNode<TConditionType>(conditionNodeDataModel.LogicalOperator, childConditionNodes);
            }
        }

        private static IConditionNode<TConditionType> CreateValueConditionNode(ValueConditionNodeDataModel conditionNodeDataModel)
        {
            TConditionType integrationTestsConditionType = MongoDbProviderRulesDataSource<TContentType, TConditionType>.Parse<TConditionType>(conditionNodeDataModel.ConditionType);
            switch (conditionNodeDataModel.DataType)
            {
                case DataTypes.Integer:
                    return new IntegerConditionNode<TConditionType>(integrationTestsConditionType, conditionNodeDataModel.Operator, Convert.ToInt32(conditionNodeDataModel.Operand));

                case DataTypes.Decimal:
                    return new DecimalConditionNode<TConditionType>(integrationTestsConditionType, conditionNodeDataModel.Operator, Convert.ToDecimal(conditionNodeDataModel.Operand));

                case DataTypes.String:
                    return new StringConditionNode<TConditionType>(integrationTestsConditionType, conditionNodeDataModel.Operator, Convert.ToString(conditionNodeDataModel.Operand));

                case DataTypes.Boolean:
                    return new BooleanConditionNode<TConditionType>(integrationTestsConditionType, conditionNodeDataModel.Operator, Convert.ToBoolean(conditionNodeDataModel.Operand));

                default:
                    throw new NotSupportedException($"Unsupported data type: {conditionNodeDataModel.DataType.ToString()}.");
            }
        }

        private static T Parse<T>(string value)
            => (T)Parse(value, typeof(T));

        private static object Parse(string value, Type type)
            => type.IsEnum ? Enum.Parse(type, value) : Convert.ChangeType(value, type);
    }
}
