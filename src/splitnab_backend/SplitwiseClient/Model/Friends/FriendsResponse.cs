using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Friends
{
    public class FriendsResponse
    {
        [JsonPropertyName("friends")] public List<FriendModel>? Friends { get; set; }
    }
}
