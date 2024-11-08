namespace Rules.Framework.Providers.MongoDb.DataModel
{
    using System;
    using System.Collections.Generic;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Bson.Serialization.Options;
    using Rules.Framework;

    [BsonKnownTypes(typeof(ComposedConditionNodeDataModel), typeof(ValueConditionNodeDataModel))]
    internal class ConditionNodeDataModel
    {
        public ConditionNodeDataModel()
        {
            this.Properties = new Dictionary<string, object>(StringComparer.Ordinal);
        }

        [BsonElement(Order = 1)]
        [BsonRepresentation(BsonType.String)]
        public LogicalOperators LogicalOperator { get; set; }

        [BsonElement(Order = 2)]
        [BsonDictionaryOptions(Representation = DictionaryRepresentation.Document)]
        public IDictionary<string, object> Properties { get; set; }
    }
}