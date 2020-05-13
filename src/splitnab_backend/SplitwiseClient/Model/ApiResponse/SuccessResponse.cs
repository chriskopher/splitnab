using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.ApiResponse
{
    public class SuccessResponse
    {
        [JsonPropertyName("success")] public bool Success { get; set; }
    }
}
