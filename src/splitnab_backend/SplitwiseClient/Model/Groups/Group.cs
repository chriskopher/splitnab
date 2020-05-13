using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SplitwiseClient.Model.Users;

namespace SplitwiseClient.Model.Groups
{
    public class Group
    {
        [JsonPropertyName("id")] public int GroupId { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("members")] public List<User>? Members { get; set; }

        [JsonPropertyName("simplify_by_default")]
        public bool SimplifyByDefault { get; set; }

        [JsonPropertyName("original_debts")] public List<Debt>? OriginalDebts { get; set; }
        [JsonPropertyName("simplified_debts")] public List<Debt>? SimplifiedDebts { get; set; }
        [JsonPropertyName("whiteboard")] public string? Whiteboard { get; set; }
        [JsonPropertyName("group_type")] public string? GroupType { get; set; }
        [JsonPropertyName("invite_link")] public string? InviteLink { get; set; }
    }
}
