using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Groups
{
    public record GroupResponse
    {
        [JsonPropertyName("group")] public Group Group { get; set; }
    }
}
