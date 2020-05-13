using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Users
{
    public class UserResponse
    {
        [JsonPropertyName("user")] public User? User { get; set; }
    }
}
