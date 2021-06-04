using System.Text.Json.Serialization;

namespace YnabClient.Model.User
{
    public record UserResponse
    {
        [JsonPropertyName("data")] public UserModel Data { get; set; }
    }
}
