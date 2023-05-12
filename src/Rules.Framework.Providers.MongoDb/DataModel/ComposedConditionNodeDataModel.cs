namespace Rules.Framework.Providers.MongoDb.DataModel
{
    using System.Collections.Generic;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonDiscriminator("composed")]
    internal sealed class ComposedConditionNodeDataModel : ConditionNodeDataModel
    {
        [BsonElement(Order = 1)]
        public IEnumerable<ConditionNodeDataModel> ChildConditionNodes { get; set; }
    }
}