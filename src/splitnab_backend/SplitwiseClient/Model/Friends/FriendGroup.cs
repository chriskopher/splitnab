using System.Text.Json.Serialization;
using SplitwiseClient.Model.Users;

namespace SplitwiseClient.Model.Friends
{
    public class FriendGroup
    {
        [JsonPropertyName("group_id")] public int Id { get; set; }
        [JsonPropertyName("balance")] public Balance? Balance { get; set; }
    }
}
