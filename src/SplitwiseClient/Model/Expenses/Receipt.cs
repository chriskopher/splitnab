using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Expenses
{
    public record Receipt
    {
        [JsonPropertyName("large")] public string Large { get; set; }
        [JsonPropertyName("original")] public string Original { get; set; }
    }
}
