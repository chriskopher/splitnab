using System;
using System.Text.Json.Serialization;

namespace YnabClient.Model.Payees
{
    public class Payee
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }

        /// <summary>
        ///     If a transfer payee, the account_id to which this payee transfers to
        /// </summary>
        [JsonPropertyName("transfer_account_id")]
        public string? TransferAccountId { get; set; }

        /// <summary>
        ///     Whether or not the payee has been deleted. Deleted payees will only be included in delta requests.
        /// </summary>
        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }
    }
}