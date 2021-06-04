using System.Collections.Generic;
using System.Text.Json.Serialization;
using SplitwiseClient.Model.Users;

namespace SplitwiseClient.Model.Friends
{
    public record FriendGroup
    {
        [JsonPropertyName("group_id")] public int Id { get; set; }
        [JsonPropertyName("balance")] public List<Balance> Balance { get; set; }
    }
}
