using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using YnabClient.Model.Accounts;

namespace YnabClient.Model.Budgets
{
    public record BudgetSummary
    {
        [JsonPropertyName("id")] public Guid Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }

        /// <summary>
        ///     The last time any changes were made to the budget from either a web or mobile client
        /// </summary>
        [JsonPropertyName("last_modified_on")]
        public DateTime LastModifiedOn { get; set; }

        /// <summary>
        ///     The earliest budget month
        /// </summary>
        [JsonPropertyName("first_month")]
        public DateTime FirstMonth { get; set; }

        /// <summary>
        ///     The latest budget month
        /// </summary>
        [JsonPropertyName("last_month")]
        public DateTime LastMonth { get; set; }

        [JsonPropertyName("date_format")] public DateFormat DateFormat { get; set; }
        [JsonPropertyName("currency_format")] public CurrencyFormat CurrencyFormat { get; set; }

        /// <summary>
        ///     The budget accounts (only included if include_accounts=true specified as query parameter)
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("accounts")]
        public List<Account> Accounts { get; set; }
    }
}
