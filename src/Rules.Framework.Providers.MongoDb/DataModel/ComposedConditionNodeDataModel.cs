
namespace Rules.Framework.Providers.MongoDb.DataModel
{
    using System.Collections.Generic;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonDiscriminator("composed")]
    internal class ComposedConditionNodeDataModel : ConditionNodeDataModel
    {
        public IEnumerable<ConditionNodeDataModel> ChildConditionNodes { get; set; }
    }
}
