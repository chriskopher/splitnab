using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Friends
{
    public record FriendResponse
    {
        [JsonPropertyName("friend")] public FriendModel Friend { get; set; }
    }
}
