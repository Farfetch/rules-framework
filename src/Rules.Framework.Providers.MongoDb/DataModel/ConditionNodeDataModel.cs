
namespace Rules.Framework.Providers.MongoDb.DataModel
{
    using MongoDB.Bson.Serialization.Attributes;
    using Rules.Framework.Core;

    [BsonKnownTypes(typeof(ComposedConditionNodeDataModel), typeof(ValueConditionNodeDataModel))]
    internal class ConditionNodeDataModel
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public LogicalOperators LogicalOperator { get; set; }
    }
}
