using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Users
{
    /// <summary>
    ///     Notification preferences.
    /// </summary>
    public record Notifications
    {
        [JsonPropertyName("added_as_friend")] public bool AddedAsFriend { get; set; }
        [JsonPropertyName("added_to_group")] public bool AddedToGroup { get; set; }
        [JsonPropertyName("expense_added")] public bool ExpenseAdded { get; set; }
        [JsonPropertyName("expense_updated")] public bool ExpenseUpdated { get; set; }
        [JsonPropertyName("bills")] public bool Bills { get; set; }
        [JsonPropertyName("payments")] public bool Payments { get; set; }
        [JsonPropertyName("monthly_summary")] public bool MonthlySummary { get; set; }
        [JsonPropertyName("announcements")] public bool Announcements { get; set; }
    }
}
