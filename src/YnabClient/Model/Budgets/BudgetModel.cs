using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YnabClient.Model.Budgets
{
    public class BudgetModel
    {
        [JsonPropertyName("budgets")] public List<BudgetSummary> Budgets { get; set; }
        [JsonPropertyName("default_budget")] public BudgetSummary DefaultBudget { get; set; }
    }
}
