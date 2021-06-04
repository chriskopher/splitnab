using System.Text.Json.Serialization;

namespace YnabClient.Model.Payees
{
    public record PayeesResponse
    {
        [JsonPropertyName("data")] public PayeeModel Data { get; set; }
    }
}
