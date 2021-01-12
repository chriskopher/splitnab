using System.Text.Json.Serialization;

namespace YnabClient.Model.User
{
    public class UserResponse
    {
        [JsonPropertyName("data")] public UserModel Data { get; set; }
    }
}
