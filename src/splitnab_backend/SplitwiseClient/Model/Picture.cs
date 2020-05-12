using System.Text.Json.Serialization;

namespace SplitwiseClient.Model
{
    /// <summary>
    /// Image URLs
    /// </summary>
    public class Picture
    {
        [JsonPropertyName("small")] public string Small { get; set; }
        [JsonPropertyName("medium")] public string Medium { get; set; }
        [JsonPropertyName("large")] public string Large { get; set; }
    }
}
