using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SplitwiseClient.Model
{
    public class Friends
    {
        [JsonPropertyName("friends")] public List<Friend> FriendsList { get; set; }
    }
    public class Friend : UserInfo
    {
        [JsonPropertyName("balance")] public List<Balance>? Balance { get; set; }
        [JsonPropertyName("groups")] public List<Group>? Groups { get; set; }
        [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
    }

    public class Balance
    {
        [JsonPropertyName("currency_code")] public string? CurrencyCode { get; set; }
        [JsonPropertyName("amount")] public decimal Amount { get; set; }
    }

    public class Group
    {
        [JsonPropertyName("group_id")] public int GroupId { get; set; }
        [JsonPropertyName("balance")] public List<Balance>? Balance  { get; set; }
    }
}
