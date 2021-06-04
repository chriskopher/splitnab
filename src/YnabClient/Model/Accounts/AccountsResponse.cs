using System.Text.Json.Serialization;

namespace YnabClient.Model.Accounts
{
    public record AccountsResponse
    {
        [JsonPropertyName("data")] public AccountsModel Data { get; set; }
    }
}
