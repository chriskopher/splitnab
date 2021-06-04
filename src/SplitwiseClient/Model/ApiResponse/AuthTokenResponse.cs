using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.ApiResponse
{
    public record AccessToken
    {
        [JsonPropertyName("access_token")] public string Token { get; set; }
        [JsonPropertyName("token_type")] public string Type { get; set; }
    }
}
