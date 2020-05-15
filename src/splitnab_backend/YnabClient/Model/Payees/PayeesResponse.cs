using System.Text.Json.Serialization;

namespace YnabClient.Model.Payees
{
    public class PayeesResponse
    {
        [JsonPropertyName("data")] public PayeeModel? Data { get; set; }
    }
}
