using System;
using System.Globalization;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers.SystemTextJson;
using YnabClient.Model.Accounts;
using YnabClient.Model.Budgets;
using YnabClient.Model.Categories;
using YnabClient.Model.Transactions;
using YnabClient.Model.User;

namespace YnabClient
{
    /// <inheritdoc />
    public class Client : IYnabClient
    {
        private const string Api = "https://api.youneedabudget.com/v1";

        private readonly IRestClient _restClient;

        public Client(IRestClient restClient)
        {
            _restClient = restClient;
            if (_restClient == null)
            {
                throw new ArgumentNullException(nameof(restClient));
            }

            _restClient.BaseUrl = new Uri(Api);
            _restClient.UseSystemTextJson();
        }

        public void ConfigureAuthorization(string personalAccessToken)
        {
            _restClient.AddDefaultHeader("Authorization", $"Bearer {personalAccessToken}");
        }

        public Task<UserResponse> GetCurrentUser()
        {
            var req = new RestRequest($"{Api}/user", DataFormat.Json);

            return _restClient.GetAsync<UserResponse>(req);
        }

        public async Task<BudgetSummaryResponse> GetBudgets(bool? includeAccounts = null)
        {
            var req = new RestRequest($"{Api}/budgets", DataFormat.Json);
            if (includeAccounts.HasValue)
            {
                req.AddQueryParameter("include_accounts", includeAccounts.Value.ToString());
            }

            return await _restClient.GetAsync<BudgetSummaryResponse>(req);
        }

        public async Task<AccountsResponse> GetBudgetAccounts(Guid budgetId, long? lastKnowledgeOfServer = null)
        {
            var req = new RestRequest($"{Api}/budgets/{budgetId}/accounts", DataFormat.Json);
            if (lastKnowledgeOfServer.HasValue)
            {
                req.AddQueryParameter("last_knowledge_of_server",
                    lastKnowledgeOfServer.Value.ToString(CultureInfo.InvariantCulture));
            }

            return await _restClient.GetAsync<AccountsResponse>(req);
        }

        public async Task<CategoriesResponse> GetBudgetCategories(Guid budgetId, long? lastKnowledgeOfServer = null)
        {
            var req = new RestRequest($"{Api}/budgets/{budgetId}/categories", DataFormat.Json);
            if (lastKnowledgeOfServer.HasValue)
            {
                req.AddQueryParameter("last_knowledge_of_server",
                    lastKnowledgeOfServer.Value.ToString(CultureInfo.InvariantCulture));
            }

            return await _restClient.GetAsync<CategoriesResponse>(req);
        }

        public async Task<SaveTransactionsResponse> PostTransactions(Guid budgetId, Transactions data)
        {
            var req = new RestRequest($"{Api}/budgets/{budgetId}/transactions", DataFormat.Json);
            req.AddJsonBody(data);

            return await _restClient.PostAsync<SaveTransactionsResponse>(req);
        }
    }
}
