namespace Rules.Framework.Providers.MongoDb.DataModel
{
    using System;
    using System.Collections.Generic;
    using MongoDB.Bson.Serialization.Attributes;
    using Rules.Framework.Core;

    [BsonKnownTypes(typeof(ComposedConditionNodeDataModel), typeof(ValueConditionNodeDataModel))]
    internal class ConditionNodeDataModel
    {
        public ConditionNodeDataModel()
        {
            this.Properties = new Dictionary<string, object>(StringComparer.Ordinal);
        }

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public LogicalOperators LogicalOperator { get; set; }

        [BsonDictionaryOptions(Representation = MongoDB.Bson.Serialization.Options.DictionaryRepresentation.Document)]
        public IDictionary<string, object> Properties { get; set; }
    }
}