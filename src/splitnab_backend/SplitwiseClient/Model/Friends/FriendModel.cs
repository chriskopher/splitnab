using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SplitwiseClient.Model.Users;

namespace SplitwiseClient.Model.Friends
{
    public class FriendModel
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("first_name")] public string FirstName { get; set; }
        [JsonPropertyName("last_name")] public string LastName { get; set; }
        [JsonPropertyName("email")] public string Email { get; set; }

        /// <summary>
        ///     'dummy', 'invited', or 'confirmed'.
        /// </summary>
        [JsonPropertyName("registration_status")]
        public string RegistrationStatus { get; set; }

        [JsonPropertyName("picture")] public Picture Picture { get; set; }
        [JsonPropertyName("balance")] public List<Balance> Balance { get; set; }

        /// <summary>
        ///     Group objects only include group balances with that friend
        /// </summary>
        [JsonPropertyName("groups")]
        public List<FriendGroup> Groups { get; set; }

        [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
    }
}
