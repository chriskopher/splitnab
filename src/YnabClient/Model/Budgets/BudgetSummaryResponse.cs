using System.Text.Json.Serialization;

namespace YnabClient.Model.Budgets
{
    public record BudgetSummaryResponse
    {
        [JsonPropertyName("data")] public BudgetModel Data { get; set; }
    }
}
