using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YnabClient.Model.Payees
{
    public class PayeeModel
    {
        [JsonPropertyName("payees")] public List<Payee> Payees { get; set; }
        [JsonPropertyName("server_knowledge")] public long ServerKnowledge { get; set; }
    }
}
