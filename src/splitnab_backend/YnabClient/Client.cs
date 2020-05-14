using System;
using System.Globalization;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers.SystemTextJson;
using YnabClient.Model.Accounts;
using YnabClient.Model.Budgets;
using YnabClient.Model.User;

namespace YnabClient
{
    public class Client
    {
        private const string Api = "https://api.youneedabudget.com/v1";

        private readonly RestClient _client = new RestClient(Api);

        public Client(string personalAccessToken)
        {
            _client.AddDefaultHeader("Authorization", $"Bearer {personalAccessToken}");
            _client.UseSystemTextJson();
        }

        /// <summary>
        ///     Returns authenticated user information.
        /// </summary>
        /// <returns>User info</returns>
        public Task<UserResponse> GetCurrentUser()
        {
            var req = new RestRequest($"{Api}/user", DataFormat.Json);

            return _client.GetAsync<UserResponse>(req);
        }

        /// <summary>
        ///     Returns budgets list with summary information
        /// </summary>
        /// <param name="includeAccounts">include_accounts query parameter - Whether to include the list of budget accounts</param>
        /// <returns>List budgets</returns>
        public Task<BudgetSummaryResponse> GetBudgets(bool? includeAccounts = null)
        {
            var req = new RestRequest($"{Api}/budgets", DataFormat.Json);
            if (includeAccounts.HasValue)
            {
                req.AddQueryParameter("include_accounts", includeAccounts.Value.ToString());
            }

            return _client.GetAsync<BudgetSummaryResponse>(req);
        }

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
        /// <returns>Account list</returns>
        public Task<AccountsResponse> GetBudgetAccounts(Guid budgetId, long? lastKnowledgeOfServer = null)
        {
            var req = new RestRequest($"{Api}/budgets/{budgetId}/accounts", DataFormat.Json);
            if (lastKnowledgeOfServer.HasValue)
            {
                req.AddQueryParameter("last_knowledge_of_server",
                    lastKnowledgeOfServer.Value.ToString(CultureInfo.InvariantCulture));
            }

            return _client.GetAsync<AccountsResponse>(req);
        }
    }
}
