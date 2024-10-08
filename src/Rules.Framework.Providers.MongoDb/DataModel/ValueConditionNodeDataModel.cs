namespace Rules.Framework.Providers.MongoDb.DataModel
{
    using MongoDB.Bson.Serialization.Attributes;
    using Rules.Framework;

    [BsonDiscriminator("value")]
    internal sealed class ValueConditionNodeDataModel : ConditionNodeDataModel
    {
        [BsonElement(Order = 1)]
        public string Condition { get; set; }

        [BsonElement(Order = 2)]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public DataTypes DataType { get; set; }

        [BsonElement(Order = 4)]
        public object Operand { get; set; }

        [BsonElement(Order = 3)]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Operators Operator { get; set; }
    }
}