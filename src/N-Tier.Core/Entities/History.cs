using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace N_Tier.Core.Entities;

public class History<T>
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public int? Id { get; set; }
    public string Action { get; set; }
    public int PrimaryKey { get; set; }
    [BsonElement("item")]
    [JsonPropertyName("item")]
    public T DbObject { get; set; }
}
