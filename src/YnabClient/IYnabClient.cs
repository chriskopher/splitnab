using System;
using System.Threading.Tasks;
using YnabClient.Model.Accounts;
using YnabClient.Model.Budgets;
using YnabClient.Model.Categories;
using YnabClient.Model.Transactions;
using YnabClient.Model.User;

namespace YnabClient
{
    /// <summary>
    /// Interface for interacting with YNAB.
    /// </summary>
    public interface IYnabClient
    {
        /// <summary>
        ///     Setup client with proper authorization header.
        /// </summary>
        /// <param name="personalAccessToken">The YNAB personal access token to use in Authorization header.</param>
        public void ConfigureAuthorization(string personalAccessToken);
        
        /// <summary>
        ///     Returns authenticated user information.
        /// </summary>
        /// <returns>The current user's info as a <see cref="UserResponse"/>.</returns>
        public Task<UserResponse> GetCurrentUser();

        /// <summary>
        ///     Returns budgets list with summary information
        /// </summary>
        /// <param name="includeAccounts">include_accounts query parameter - Whether to include the list of budget accounts</param>
        /// <returns>The current user's budgets as a <see cref="BudgetSummaryResponse"/>.</returns>
        public Task<BudgetSummaryResponse> GetBudgets(bool? includeAccounts = null);

        /// <summary>
        ///     Returns all accounts for the specified budget.
        /// </summary>
        /// <param name="budgetId">
        ///     The id of the budget (“last-used” can be used to specify the last used budget and “default” can
        ///     be used if default budget selection is enabled (see: https://api.youneedabudget.com/#oauth-default-budget)
        /// </param>
        /// <param name="lastKnowledgeOfServer">
        ///     last_knowledge_of_server query parameter - The starting server knowledge. If
        ///     provided, only entities that have changed since last_knowledge_of_server will be included.
        /// </param>
        /// <returns>The specified budget's account as a <see cref="AccountsResponse"/>.</returns>
        public Task<AccountsResponse> GetBudgetAccounts(Guid budgetId, long? lastKnowledgeOfServer = null);

        /// <summary>
        ///     Returns all categories grouped by category group. Amounts (budgeted, activity, balance, etc.) are specific to the
        ///     current budget month (UTC).
        /// </summary>
        /// <param name="budgetId">
        ///     The id of the budget (“last-used” can be used to specify the last used budget and “default” can
        ///     be used if default budget selection is enabled (see: https://api.youneedabudget.com/#oauth-default-budget)
        /// </param>
        /// <param name="lastKnowledgeOfServer">
        ///     last_knowledge_of_server query parameter - The starting server knowledge. If
        ///     provided, only entities that have changed since last_knowledge_of_server will be included.
        /// </param>
        /// <returns>THe specified budget's categories as a <see cref="CategoriesResponse"/>.</returns>
        public Task<CategoriesResponse> GetBudgetCategories(Guid budgetId, long? lastKnowledgeOfServer = null);

        /// <summary>
        ///     Creates a single transaction or multiple transactions. If you provide a body containing a transaction object,
        ///     single transaction will be created and if you provide a body containing a transactions array, multiple transactions
        ///     will be created. Scheduled transactions cannot be created on this endpoint.
        /// </summary>
        /// <param name="budgetId">
        ///     The id of the budget (“last-used” can be used to specify the last used budget and “default” can
        ///     be used if default budget selection is enabled (see: https://api.youneedabudget.com/#oauth-default-budget)
        /// </param>
        /// <param name="data">
        ///     The transaction or transactions to create. To create a single transaction you can specify a value
        ///     for the transaction object and to create multiple transactions you can specify an array of transactions. It is
        ///     expected that you will only provide a value for one of these objects.
        /// </param>
        /// <returns>A response of the saved transactions as a <see cref="SaveTransactionsResponse"/>.</returns>
        public Task<SaveTransactionsResponse> PostTransactions(Guid budgetId, Transactions data);
    }
}
