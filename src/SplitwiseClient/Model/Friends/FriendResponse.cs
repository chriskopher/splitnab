using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Friends
{
    public class FriendResponse
    {
        [JsonPropertyName("friend")] public FriendModel Friend { get; set; }
    }
}
