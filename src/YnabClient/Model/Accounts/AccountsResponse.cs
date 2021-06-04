using System.Text.Json.Serialization;

namespace YnabClient.Model.Accounts
{
    public class AccountsResponse
    {
        [JsonPropertyName("data")] public AccountsModel Data { get; set; }
    }
}
