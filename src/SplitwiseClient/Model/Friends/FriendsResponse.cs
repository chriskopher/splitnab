using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Friends
{
    public record FriendsResponse
    {
        [JsonPropertyName("friends")] public List<FriendModel> Friends { get; set; }
    }
}
