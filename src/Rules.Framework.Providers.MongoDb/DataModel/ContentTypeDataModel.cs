namespace Rules.Framework.Providers.MongoDb.DataModel
{
    using System;
    using MongoDB.Bson.Serialization.Attributes;

    internal class ContentTypeDataModel
    {
        [BsonElement("creation", Order = 3)]
        public DateTime Creation { get; set; }

        [BsonElement("id", Order = 1)]
        public Guid Id { get; set; }

        [BsonElement("name", Order = 2)]
        public string Name { get; set; }
    }
}