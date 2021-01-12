using System.Text.Json.Serialization;

namespace YnabClient.Model.User
{
    public class UserModel
    {
        [JsonPropertyName("user")] public User User { get; set; }
    }
}
