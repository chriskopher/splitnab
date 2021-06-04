using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Users
{
    public record CurrentUserResponse
    {
        [JsonPropertyName("user")] public User User { get; set; }
    }
}
