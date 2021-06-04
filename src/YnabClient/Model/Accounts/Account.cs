using System;
using System.Text.Json.Serialization;

namespace YnabClient.Model.Accounts
{
    public class Account
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }

        /// <summary>
        ///     The type of account. Note: payPal, merchantAccount, investmentAccount, and mortgage types have been deprecated and
        ///     will be removed in the future.
        ///     Enum: [ checking, savings, cash, creditCard, lineOfCredit, otherAsset, otherLiability, payPal, merchantAccount,
        ///     investmentAccount, mortgage ]
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        ///     Whether this account is on budget or not
        /// </summary>
        [JsonPropertyName("on_budget")]
        public bool OnBudget { get; set; }

        /// <summary>
        ///     Whether this account is closed or not
        /// </summary>
        [JsonPropertyName("closed")]
        public bool Closed { get; set; }

        [JsonPropertyName("note")] public string Note { get; set; }

        /// <summary>
        ///     The current balance of the account in milliunits format
        /// </summary>
        [JsonPropertyName("balance")]
        public long? Balance { get; set; }

        /// <summary>
        ///     The current cleared balance of the account in milliunits format
        /// </summary>
        [JsonPropertyName("cleared_balance")]
        public long? ClearedBalance { get; set; }

        /// <summary>
        ///     The current uncleared balance of the account in milliunits format
        /// </summary>
        [JsonPropertyName("uncleared_balance")]
        public long? UnclearedBalance { get; set; }

        /// <summary>
        ///     The payee id which should be used when transferring to this account
        /// </summary>
        [JsonPropertyName("transfer_payee_id")]
        public string TransferPayeeId { get; set; }

        /// <summary>
        ///     Whether or not the account has been deleted. Deleted accounts will only be included in delta requests.
        /// </summary>
        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }
    }
}
