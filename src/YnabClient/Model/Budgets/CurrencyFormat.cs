using System.Text.Json.Serialization;

namespace YnabClient.Model.Budgets
{
    /// <summary>
    ///     The currency format setting for the budget. In some cases the format will not be available and will be specified as
    ///     null.
    /// </summary>
    public class CurrencyFormat
    {
        [JsonPropertyName("iso_code")] public string IsoCode { get; set; }
        [JsonPropertyName("example_format")] public string ExampleFormat { get; set; }
        [JsonPropertyName("decimal_digits")] public int DecimalDigits { get; set; }

        [JsonPropertyName("decimal_separator")]
        public string DecimalSeparator { get; set; }

        [JsonPropertyName("symbol_first")] public bool SymbolFirst { get; set; }
        [JsonPropertyName("group_separator")] public string GroupSeparator { get; set; }
        [JsonPropertyName("currency_symbol")] public string CurrencySymbol { get; set; }
        [JsonPropertyName("display_symbol")] public bool DisplaySymbol { get; set; }
    }
}
