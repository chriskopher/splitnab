using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers.SystemTextJson;
using SplitwiseClient.Model.ApiResponse;
using SplitwiseClient.Model.Expenses;
using SplitwiseClient.Model.Friends;
using SplitwiseClient.Model.Users;

namespace SplitwiseClient
{
    /// <inheritdoc />
    public class Client : ISplitwiseClient
    {
        private const string Url = "https://secure.splitwise.com";
        private const string Api = Url + "/api/v3.0";

        private readonly IRestClient _restClient;

        public Client(IRestClient restClient)
        {
            _restClient = restClient;
            if (_restClient == null)
            {
                throw new ArgumentNullException(nameof(restClient));
            }

            _restClient.BaseUrl = new Uri(Url);
            _restClient.UseSystemTextJson();
        }

        public async Task ConfigureAccessToken(string consumerKey, string consumerSecret)
        {
            SetAuthorizationToken(await GetAuthorizationToken(consumerKey, consumerSecret));
        }

        /// <summary>
        ///     Get an access token from the OAuth server.
        /// </summary>
        /// <param name="consumerKey">The Splitwise application consumer key.</param>
        /// <param name="consumerSecret">The Splitwise application consumer secret.</param>
        /// <returns>The access token.</returns>
        private async Task<AccessToken> GetAuthorizationToken(string consumerKey, string consumerSecret)
        {
            var req = new RestRequest("oauth/token");
            req.AddHeader("Accept", "application/json");
            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            req.AddParameter("client_id", consumerKey);
            req.AddParameter("client_secret", consumerSecret);
            req.AddParameter("grant_type", "client_credentials");

            return await _restClient.PostAsync<AccessToken>(req);
        }

        /// <summary>
        ///     Set the default access token to be used on each request. If the token is null, no action is performed.
        /// </summary>
        /// <param name="token">The access token to be set as the default</param>
        private void SetAuthorizationToken(AccessToken token)
        {
            if (_restClient.DefaultParameters.All(x => x.Name != "Authorization"))
            {
                _restClient.AddDefaultHeader("Authorization", $"Bearer {token.Token}");
            }
        }

        public async Task<CurrentUserResponse> GetCurrentUser()
        {
            var req = new RestRequest($"{Api}/get_current_user", DataFormat.Json);
            return await _restClient.GetAsync<CurrentUserResponse>(req);
        }

        public async Task<FriendsResponse> GetFriends()
        {
            var req = new RestRequest($"{Api}/get_friends", DataFormat.Json);
            return await _restClient.GetAsync<FriendsResponse>(req);
        }

        public async Task<ExpensesResponse> GetExpenses(int? groupId = null, int? friendId = null,
            DateTimeOffset? datedAfter = null, DateTimeOffset? datedBefore = null, DateTimeOffset? updatedAfter = null,
            DateTimeOffset? updatedBefore = null, int limit = 20, int offset = 0)
        {
            var req = new RestRequest($"{Api}/get_expenses");

            if (groupId.HasValue)
            {
                req.AddQueryParameter("group_id", groupId.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (friendId.HasValue)
            {
                req.AddQueryParameter("friend_id", friendId.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (datedAfter.HasValue)
            {
                req.AddQueryParameter("dated_after", datedAfter.Value.ToString("o", CultureInfo.InvariantCulture));
            }

            if (datedBefore.HasValue)
            {
                req.AddQueryParameter("dated_before", datedBefore.Value.ToString("o", CultureInfo.InvariantCulture));
            }

            if (updatedAfter.HasValue)
            {
                req.AddQueryParameter("updated_after", updatedAfter.Value.ToString("o", CultureInfo.InvariantCulture));
            }

            if (updatedBefore.HasValue)
            {
                req.AddQueryParameter("updated_before",
                    updatedBefore.Value.ToString("o", CultureInfo.InvariantCulture));
            }

            if (limit != 20)
            {
                req.AddQueryParameter("limit", limit.ToString(CultureInfo.InvariantCulture));
            }

            if (offset != 0)
            {
                req.AddQueryParameter("offset", offset.ToString(CultureInfo.InvariantCulture));
            }

            return await _restClient.GetAsync<ExpensesResponse>(req);
        }
    }
}
