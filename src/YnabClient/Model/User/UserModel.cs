using System.Text.Json.Serialization;

namespace YnabClient.Model.User
{
    public record UserModel
    {
        [JsonPropertyName("user")] public User User { get; set; }
    }
}
