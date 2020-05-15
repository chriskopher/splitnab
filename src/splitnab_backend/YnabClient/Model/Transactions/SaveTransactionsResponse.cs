using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YnabClient.Model.Transactions
{
    public class SaveTransactionsResponse
    {
        [JsonPropertyName("data")] public SaveTransactionsModel? Data { get; set; }
    }

    public class SaveTransactionsModel
    {
        /// <summary>
        /// The transaction ids that were saved
        /// </summary>
        [JsonPropertyName("transaction_ids")]
        public List<string>? TransactionIds { get; set; }

        /// <summary>
        /// The a single transaction was specified, the transaction that was saved
        /// </summary>
        [JsonPropertyName("transaction")]
        public TransactionDetail? Transaction { get; set; }

        /// <summary>
        /// If multiple transactions were specified, the transactions that were saved
        /// </summary>
        [JsonPropertyName("transactions")]
        public List<TransactionDetail>? Transactions { get; set; }

        /// <summary>
        /// If multiple transactions were specified, a list of import_ids that were not created because of an existing import_id found on the same account
        /// </summary>
        [JsonPropertyName("duplicate_import_ids")]
        public List<string>? DuplicateImportIds { get; set; }

        /// <summary>
        /// The knowledge of the server
        /// </summary>
        [JsonPropertyName("server_knowledge")]
        public long ServerKnowledge { get; set; }
    }

    public class TransactionDetail
    {
        [JsonPropertyName("id")] public string? Id { get; set; }

        /// <summary>
        /// The transaction date in ISO format (e.g. 2016-12-01).
        /// </summary>
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// The transaction amount in milliunits format.
        /// </summary>
        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        /// <summary>
        /// Max Length: 200
        /// </summary>
        [JsonPropertyName("memo")]
        public string? Memo { get; set; }

        /// <summary>
        /// The cleared status of the transaction
        /// Enum: [ cleared, uncleared, reconciled ]
        /// </summary>
        [JsonPropertyName("cleared")]
        public string? Cleared { get; set; }

        /// <summary>
        /// Whether or not the transaction is approved. If not supplied, transaction will be unapproved by default.
        /// </summary>
        [JsonPropertyName("approved")]
        public bool? Approved { get; set; }

        /// <summary>
        /// The transaction flag
        /// Enum: [ red, orange, yellow, green, blue, purple, ]
        /// </summary>
        [JsonPropertyName("flag_color")]
        public string? FlagColor { get; set; }

        [JsonPropertyName("account_id")] public Guid AccountId { get; set; }

        /// <summary>
        /// The payee for the subtransaction.
        /// </summary>
        [JsonPropertyName("payee_id")]
        public Guid? PayeeId { get; set; }

        /// <summary>
        /// The category for the subtransaction. Credit Card Payment categories are not permitted and will be ignored if supplied.
        /// </summary>
        [JsonPropertyName("category_id")]
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// If a transfer transaction, the account to which it transfers
        /// </summary>
        [JsonPropertyName("transfer_account_id")]
        public Guid? TransferAccountId { get; set; }

        /// <summary>
        /// If a transfer transaction, the id of transaction on the other side of the transfer
        /// </summary>
        [JsonPropertyName("transfer_transaction_id")]
        public string? TransferTransactionId { get; set; }

        /// <summary>
        /// If transaction is matched, the id of the matched transaction
        /// </summary>
        [JsonPropertyName("matched_transaction_id")]
        public string? MatchedTransactionId { get; set; }

        /// <summary>
        ///  If the Transaction was imported, this field is a unique (by account) import identifier. If this transaction was imported through File Based Import or Direct Import and not through the API, the import_id will have the format: 'YNAB:[milliunit_amount]:[iso_date]:[occurrence]'. For example, a transaction dated 2015-12-30 in the amount of -$294.23 USD would have an import_id of 'YNAB:-294230:2015-12-30:1’. If a second transaction on the same account was imported and had the same date and same amount, its import_id would be 'YNAB:-294230:2015-12-30:2’.
        /// </summary>
        [JsonPropertyName("import_id")]
        public string? ImportId { get; set; }

        /// <summary>
        /// Whether or not the transaction has been deleted. Deleted transactions will only be included in delta requests.
        /// </summary>
        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("account_name")] public string? AccountName { get; set; }
        [JsonPropertyName("payee_name")] public string? PayeeName { get; set; }
        [JsonPropertyName("category_name")] public string? CategoryName { get; set; }

        /// <summary>
        /// If a split transaction, the subtransactions.
        /// </summary>
        [JsonPropertyName("subtransactions")]
        public List<SaveSubTransaction>? SubTransactions { get; set; }
    }
}
