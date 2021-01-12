using System.Text.Json.Serialization;

namespace YnabClient.Model.Budgets
{
    public class BudgetSummaryResponse
    {
        [JsonPropertyName("data")] public BudgetModel Data { get; set; }
    }
}
