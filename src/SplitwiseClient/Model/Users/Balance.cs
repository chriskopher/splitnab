using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Users
{
    public record Balance
    {
        [JsonPropertyName("currency_code")] public string CurrencyCode { get; set; }
        [JsonPropertyName("amount")] public string Amount { get; set; }
    }
}
