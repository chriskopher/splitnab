using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Users
{
    public class CurrentUserResponse
    {
        [JsonPropertyName("user")] public User User { get; set; }
    }
}
