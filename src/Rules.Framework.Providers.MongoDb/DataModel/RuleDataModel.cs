namespace Rules.Framework.Providers.MongoDb.DataModel
{
    using System;
    using MongoDB.Bson.Serialization.Attributes;

    internal sealed class RuleDataModel
    {
        [BsonElement(Order = 7)]
        public dynamic Content { get; set; }

        [BsonElement(Order = 2)]
        public string ContentType { get; set; }

        [BsonElement(Order = 3)]
        public DateTime DateBegin { get; set; }

        [BsonElement(Order = 4)]
        public DateTime? DateEnd { get; set; }

        [BsonId(Order = 1)]
        public string Name { get; set; }

        [BsonElement(Order = 5)]
        public int Priority { get; set; }

        [BsonElement(Order = 6)]
        public ConditionNodeDataModel RootCondition { get; set; }
    }
}