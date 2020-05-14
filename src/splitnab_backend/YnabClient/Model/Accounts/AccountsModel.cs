using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YnabClient.Model.Accounts
{
    public class AccountsModel
    {
        [JsonPropertyName("accounts")] public List<Account>? Accounts { get; set; }

        /// <summary>
        ///     The knowledge of the server
        /// </summary>
        [JsonPropertyName("server_knowledge")]
        public long? ServerKnowledge { get; set; }
    }
}
