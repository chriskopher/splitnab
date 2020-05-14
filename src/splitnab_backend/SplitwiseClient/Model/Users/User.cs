using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Users
{
    public class User
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("first_name")] public string? FirstName { get; set; }
        [JsonPropertyName("last_name")] public string? LastName { get; set; }
        [JsonPropertyName("picture")] public Picture? Picture { get; set; }
        [JsonPropertyName("custom_picture")] public bool? CustomPicture { get; set; }
        [JsonPropertyName("email")] public string? Email { get; set; }

        /// <summary>
        ///     'dummy', 'invited', or 'confirmed'.
        /// </summary>
        [JsonPropertyName("registration_status")]
        public string? RegistrationStatus { get; set; }

        [JsonPropertyName("force_refresh_at")] public DateTime? ForceRefreshAt { get; set; }
        [JsonPropertyName("locale")] public string? Locale { get; set; }
        [JsonPropertyName("country_code")] public string? CountryCode { get; set; }
        [JsonPropertyName("date_format")] public string? DateFormat { get; set; }
        [JsonPropertyName("default_currency")] public string? DefaultCurrency { get; set; }
        [JsonPropertyName("default_group_id")] public int? DefaultGroupId { get; set; }

        /// <summary>
        ///     The last time notifications were marked as read.
        /// </summary>
        [JsonPropertyName("notifications_read")]
        public DateTime? NotificationsRead { get; set; }

        /// <summary>
        ///     The number of unread notifications
        /// </summary>
        [JsonPropertyName("notifications_count")]
        public int? NotificationsCount { get; set; }

        [JsonPropertyName("notifications")] public Notifications? Notifications { get; set; }
    }
}
