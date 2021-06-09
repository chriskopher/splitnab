using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Splitnab.Model;
using YnabClient;

namespace Splitnab
{
    /// <inheritdoc/>
    public class GetYnabInfoOperation : IGetYnabInfoOperation
    {
        private readonly ILogger<GetYnabInfoOperation> _logger;
        private readonly IYnabClient _ynabClient;

        public GetYnabInfoOperation(ILogger<GetYnabInfoOperation> logger, IYnabClient ynabClient)
        {
            _logger = logger;
            _ynabClient = ynabClient;
        }

        public async Task<YnabInfo?> Invoke(AppSettings appSettings)
        {
            _ynabClient.ConfigureAuthorization(appSettings.Ynab.PersonalAccessToken);

            var ynabBudgets = await _ynabClient.GetBudgets(true);
            if (ynabBudgets.Data?.Budgets == null)
            {
                _logger.LogWarning("Unable to fetch YNAB budgets");

                return null;
            }

            var ynabBudget = ynabBudgets.Data.Budgets.FirstOrDefault(x => x.Name == appSettings.Ynab.BudgetName);
            if (ynabBudget == null)
            {
                _logger.LogWarning("Unable to find YNAB budget");

                return null;
            }

            var ynabBudgetAccounts = await _ynabClient.GetBudgetAccounts(ynabBudget.Id);
            var splitwiseAccount =
                ynabBudgetAccounts.Data?.Accounts?.SingleOrDefault(x =>
                    x.Name == appSettings.Ynab.SplitwiseAccountName);
            if (splitwiseAccount == null)
            {
                _logger.LogWarning(
                    "Unable to find YNAB splitwise account {SplitwiseAccountName} in budget {YnabBudgetName}",
                    appSettings.Ynab.SplitwiseAccountName, appSettings.Ynab.BudgetName);

                return null;
            }

            return new YnabInfo {Budget = ynabBudget, SplitwiseAccount = splitwiseAccount};
        }
    }
}
