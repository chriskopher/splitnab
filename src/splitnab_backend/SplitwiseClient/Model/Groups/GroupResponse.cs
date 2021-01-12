using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Groups
{
    public class GroupResponse
    {
        [JsonPropertyName("group")] public Group Group { get; set; }
    }
}
