namespace Rules.Framework.Providers.MongoDb.DataModel
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonDiscriminator("composed")]
    internal sealed class ComposedConditionNodeDataModel : ConditionNodeDataModel
    {
        [BsonElement(Order = 1)]
        public ConditionNodeDataModel[] ChildConditionNodes { get; set; }
    }
}