using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Expenses
{
    public class Receipt
    {
        [JsonPropertyName("large")] public string? Large { get; set; }
        [JsonPropertyName("original")] public string? Original { get; set; }
    }
}