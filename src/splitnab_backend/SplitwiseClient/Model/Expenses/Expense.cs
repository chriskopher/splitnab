using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SplitwiseClient.Model.Users;

namespace SplitwiseClient.Model.Expenses
{
    public class Expense
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("group_id")] public int? GroupId { get; set; }
        [JsonPropertyName("friendship_id")] public int? FriendshipId { get; set; }

        [JsonPropertyName("expense_bundle_id")]
        public int? ExpenseBundleId { get; set; }

        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("repeats")] public bool Repeats { get; set; }
        [JsonPropertyName("repeat_interval")] public string? RepeatInterval { get; set; }
        [JsonPropertyName("email_reminder")] public bool EmailReminder { get; set; }

        [JsonPropertyName("email_reminder_in_advance")]
        public int EmailReminderInAdvance { get; set; }

        [JsonPropertyName("next_repeat")] public string? NextRepeat { get; set; }
        [JsonPropertyName("details")] public string? Details { get; set; }
        [JsonPropertyName("comments_count")] public int CommentsCount { get; set; }
        [JsonPropertyName("payment")] public bool Payment { get; set; }
        [JsonPropertyName("creation_method")] public string? CreationMethod { get; set; }

        [JsonPropertyName("transaction_method")]
        public string? TransactionMethod { get; set; }

        [JsonPropertyName("transaction_confirmed")]
        public bool TransactionConfirmed { get; set; }

        [JsonPropertyName("transaction_id")] public int? TransactionId { get; set; }
        [JsonPropertyName("cost")] public string? Cost { get; set; }
        [JsonPropertyName("currency_code")] public string? CurrencyCode { get; set; }
        [JsonPropertyName("repayments")] public List<Repayment>? Repayments { get; set; }
        [JsonPropertyName("date")] public DateTime? Date { get; set; }
        [JsonPropertyName("created_at")] public DateTime? CreatedAt { get; set; }
        [JsonPropertyName("created_by")] public User? CreatedBy { get; set; }
        [JsonPropertyName("updated_at")] public DateTime? UpdatedAt { get; set; }
        [JsonPropertyName("updated_by")] public User? UpdatedBy { get; set; }
        [JsonPropertyName("deleted_at")] public DateTime? DeletedAt { get; set; }
        [JsonPropertyName("deleted_by")] public User? DeletedBy { get; set; }
        [JsonPropertyName("category")] public Category? Category { get; set; }
        [JsonPropertyName("receipt")] public Receipt? Receipt { get; set; }
        [JsonPropertyName("users")] public List<User>? Users { get; set; }
    }
}
