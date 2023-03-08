using System.Text.Json.Serialization;

namespace Fedodo.Server.Model.ActivityPub;

public class CollectionPage<T>
{
    [JsonPropertyName("@context")] public string Context { get; set; } = "https://www.w3.org/ns/activitystreams";
    [JsonPropertyName("type")] public string Type { get; set; } = "CollectionPage";
    [JsonPropertyName("items")] public IEnumerable<T> Items { get; set; }
    [JsonPropertyName("next")] public Uri Next { get; set; }
    [JsonPropertyName("prev")] public Uri Prev { get; set; }
    [JsonPropertyName("partOf")] public Uri PartOf { get; set; }
    [JsonPropertyName("id")] public Uri Id { get; set; }
}