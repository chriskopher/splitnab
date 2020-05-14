using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Expenses
{
    public class Repayment
    {
        [JsonPropertyName("from")] public int From { get; set; }
        [JsonPropertyName("to")] public int To { get; set; }
        [JsonPropertyName("amount")] public string? Amount { get; set; }
    }
}
