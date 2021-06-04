using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Expenses
{
    public record ExpensesResponse
    {
        [JsonPropertyName("expenses")] public List<Expense> Expenses { get; set; }
    }
}
