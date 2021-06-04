using System.Text.Json.Serialization;

namespace YnabClient.Model.ApiResponse
{
    public record ErrorResponse
    {
        [JsonPropertyName("error")] public ErrorDetail ErrorDetail { get; set; }
    }

    public record ErrorDetail
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("detail")] public string Detail { get; set; }
    }
}
