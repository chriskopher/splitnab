﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers.SystemTextJson;
using YnabClient.Model.Accounts;
using YnabClient.Model.Budgets;
using YnabClient.Model.Categories;
using YnabClient.Model.Payees;
using YnabClient.Model.Transactions;
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
        /// <returns>List categories</returns>
        public Task<CategoriesResponse> GetBudgetCategories(Guid budgetId, long? lastKnowledgeOfServer = null)
        {
            var req = new RestRequest($"{Api}/budgets/{budgetId}/categories", DataFormat.Json);
            if (lastKnowledgeOfServer.HasValue)
            {
                req.AddQueryParameter("last_knowledge_of_server",
                    lastKnowledgeOfServer.Value.ToString(CultureInfo.InvariantCulture));
            }

            return _client.GetAsync<CategoriesResponse>(req);
        }

        /// <summary>
        ///     Returns all payees.
        /// </summary>
        /// <param name="budgetId">
        ///     The id of the budget (“last-used” can be used to specify the last used budget and “default” can
        ///     be used if default budget selection is enabled (see: https://api.youneedabudget.com/#oauth-default-budget)
        /// </param>
        /// <param name="lastKnowledgeOfServer">
        ///     last_knowledge_of_server query parameter - The starting server knowledge. If
        ///     provided, only entities that have changed since last_knowledge_of_server will be included.
        /// </param>
        /// <returns>List payees</returns>
        public Task<PayeesResponse> GetBudgetPayees(Guid budgetId, long? lastKnowledgeOfServer = null)
        {
            var req = new RestRequest($"{Api}/budgets/{budgetId}/payees", DataFormat.Json);
            if (lastKnowledgeOfServer.HasValue)
            {
                req.AddQueryParameter("last_knowledge_of_server",
                    lastKnowledgeOfServer.Value.ToString(CultureInfo.InvariantCulture));
            }

            return _client.GetAsync<PayeesResponse>(req);
        }

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
        /// <returns></returns>
        public Task<SaveTransactionsResponse> PostTransactions(Guid budgetId, Transactions data)
        {
            var req = new RestRequest($"{Api}/budgets/{budgetId}/transactions", DataFormat.Json);
            req.AddJsonBody(data);

            return _client.PostAsync<SaveTransactionsResponse>(req);
        }
    }
}