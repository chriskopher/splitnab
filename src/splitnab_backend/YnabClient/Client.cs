using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers.SystemTextJson;
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
        /// <returns></returns>
        public Task<UserResponse> GetCurrentUser()
        {
            var req = new RestRequest($"{Api}/user", DataFormat.Json);

            return _client.GetAsync<UserResponse>(req);
        }

        /// <summary>
        ///     Returns budgets list with summary information
        /// </summary>
        /// <param name="includeAccounts">include_accounts query parameter - Whether to include the list of budget accounts</param>
        /// <returns></returns>
        public Task<BudgetSummaryResponse> GetBudgets(bool? includeAccounts = null)
        {
            var req = new RestRequest($"{Api}/budgets", DataFormat.Json);
            if (includeAccounts.HasValue)
            {
                req.AddQueryParameter("include_accounts", includeAccounts.Value.ToString());
            }

            return _client.GetAsync<BudgetSummaryResponse>(req);
        }
    }
}
