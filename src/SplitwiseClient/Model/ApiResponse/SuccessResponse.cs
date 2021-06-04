using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.ApiResponse
{
    public record SuccessResponse
    {
        [JsonPropertyName("success")] public bool Success { get; set; }
    }
}
