using YnabClient.Model.Accounts;
using YnabClient.Model.Budgets;

namespace Splitnab.Model
{
    public record YnabInfo
    {
        public BudgetSummary Budget { get; init; }
        public Account SplitwiseAccount { get; init; }
    }
}
