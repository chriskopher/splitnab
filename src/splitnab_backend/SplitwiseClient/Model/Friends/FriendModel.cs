using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SplitwiseClient.Model.Users;

namespace SplitwiseClient.Model.Friends
{
    public class FriendModel : User
    {
        /// <summary>
        ///     Group objects only include group balances with that friend
        /// </summary>
        [JsonPropertyName("groups")]
        public List<FriendGroup>? Groups { get; set;}

        [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; }
    }
}
