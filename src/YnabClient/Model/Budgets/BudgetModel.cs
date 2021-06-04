using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YnabClient.Model.Budgets
{
    public record BudgetModel
    {
        [JsonPropertyName("budgets")] public List<BudgetSummary> Budgets { get; set; }
        [JsonPropertyName("default_budget")] public BudgetSummary DefaultBudget { get; set; }
    }
}
