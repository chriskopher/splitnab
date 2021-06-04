using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Expenses
{
    public record Category
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
    }
}
