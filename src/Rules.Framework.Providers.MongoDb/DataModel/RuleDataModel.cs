namespace Rules.Framework.Providers.MongoDb.DataModel
{
    using System;
    using MongoDB.Bson.Serialization.Attributes;

    internal class RuleDataModel
    {
        public dynamic Content { get; set; }

        public string ContentType { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        [BsonId]
        public string Name { get; set; }

        public int Priority { get; set; }

        public ConditionNodeDataModel RootCondition { get; set; }
    }
}