using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Users
{
    /// <summary>
    ///     Image URLs
    /// </summary>
    public record Picture
    {
        [JsonPropertyName("small")] public string Small { get; set; }
        [JsonPropertyName("medium")] public string Medium { get; set; }
        [JsonPropertyName("large")] public string Large { get; set; }
    }
}
