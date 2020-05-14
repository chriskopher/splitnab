using System;
using System.Text.Json.Serialization;

namespace YnabClient.Model.User
{
    public class UserResponse
    {
        [JsonPropertyName("data")] public UserModel? Data { get; set; }
    }

    public class UserModel
    {
        [JsonPropertyName("user")] public User? User { get; set; }
    }

    public class User
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }
    }
}
