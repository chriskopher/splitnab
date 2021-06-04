using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Groups
{
    public record Debt
    {
        [JsonPropertyName("from")] public int? From { get; set; }
        [JsonPropertyName("to")] public int? To { get; set; }
        [JsonPropertyName("currency_code")] public string CurrencyCode { get; set; }
        [JsonPropertyName("amount")] public decimal Amount { get; set; }
    }
}
