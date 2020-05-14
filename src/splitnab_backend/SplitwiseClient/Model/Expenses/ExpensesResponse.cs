using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SplitwiseClient.Model.Expenses
{
    public class ExpensesResponse
    {
        [JsonPropertyName("expenses")] public List<Expense>? Expenses { get; set; }
    }
}
