using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YnabClient.Model.Transactions
{
    public class Transactions
    {
        [JsonPropertyName("transactions")] public List<SaveTransaction>? SaveTransactions { get; set; }
    }

    public class SaveTransaction
    {
        [JsonPropertyName("account_id")] public Guid AccountId { get; set; }

        /// <summary>
        /// The transaction date in ISO format (e.g. 2016-12-01). Future dates (scheduled transactions) are not permitted. Split transaction dates cannot be changed and if a different date is supplied it will be ignored.
        /// </summary>
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// The transaction amount in milliunits format. Split transaction amounts cannot be changed and if a different amount is supplied it will be ignored.
        /// </summary>
        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        /// <summary>
        /// The payee for the transaction. To create a transfer between two accounts, use the account transfer payee pointing to the target account. Account transfer payees are specified as tranfer_payee_id on the account resource.
        /// </summary>
        [JsonPropertyName("payee_id")]
        public Guid? PayeeId { get; set; }

        /// <summary>
        /// The payee name. If a payee_name value is provided and payee_id has a null value, the payee_name value will be used to resolve the payee by either (1) a matching payee rename rule (only if import_id is also specified) or (2) a payee with the same name or (3) creation of a new payee.
        /// Max Length: 50
        /// </summary>
        [JsonPropertyName("payee_name")]
        public string? PayeeName { get; set; }

        /// <summary>
        /// The category for the transaction. To configure a split transaction, you can specify null for category_id and provide a subtransactions array as part of the transaction object. If an existing transaction is a split, the category_id cannot be changed. Credit Card Payment categories are not permitted and will be ignored if supplied.
        /// </summary>
        [JsonPropertyName("category_id")]
        public Guid? CategoryId { get; set; }

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

        /// <summary>
        /// maxLength: 36
        /// If specified, the new transaction will be assigned this import_id and considered "imported". We will also attempt to match this imported transaction to an existing “user-entered” transaction on the same account, with the same amount, and with a date +/-10 days from the imported transaction date.
        ///
        /// Transactions imported through File Based Import or Direct Import (not through the API) are assigned an import_id in the format: 'YNAB:[milliunit_amount]:[iso_date]:[occurrence]'. For example, a transaction dated 2015-12-30 in the amount of -$294.23 USD would have an import_id of 'YNAB:-294230:2015-12-30:1’. If a second transaction on the same account was imported and had the same date and same amount, its import_id would be 'YNAB:-294230:2015-12-30:2’. Using a consistent format will prevent duplicates through Direct Import and File Based Import.
        ///
        /// If import_id is omitted or specified as null, the transaction will be treated as a “user-entered” transaction. As such, it will be eligible to be matched against transactions later being imported (via DI, FBI, or API).
        /// </summary>
        [JsonPropertyName("import_id")]
        public string? ImportId { get; set; }

        /// <summary>
        /// An array of subtransactions to configure a transaction as a split. Updating subtransactions on an existing split transaction is not supported.
        /// </summary>
        [JsonPropertyName("subtransactions")]
        public List<SaveSubTransaction>? SubTransactions { get; set; }
    }

    public class SaveSubTransaction
    {
        /// <summary>
        /// The subtransaction amount in milliunits format.
        /// </summary>
        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        /// <summary>
        /// The payee for the subtransaction.
        /// </summary>
        [JsonPropertyName("payee_id")]
        public Guid? PayeeId { get; set; }

        /// <summary>
        /// maxLength: 50
        /// The payee name. If a payee_name value is provided and payee_id has a null value, the payee_name value will be used to resolve the payee by either (1) a matching payee rename rule (only if import_id is also specified on parent transaction) or (2) a payee with the same name or (3) creation of a new payee.
        /// </summary>
        [JsonPropertyName("payee_name")]
        public string? PayeeName { get; set; }

        /// <summary>
        /// The category for the subtransaction. Credit Card Payment categories are not permitted and will be ignored if supplied.
        /// </summary>
        [JsonPropertyName("category_id")]
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// maxLength: 200
        /// </summary>
        [JsonPropertyName("memo")]
        public string? Memo { get; set; }
    }
}
