using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Users
{
    public record UserResponse
    {
        [JsonPropertyName("user")] public User User { get; set; }
    }
}
