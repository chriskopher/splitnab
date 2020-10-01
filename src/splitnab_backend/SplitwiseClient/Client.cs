using System;
using System.Globalization;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers.SystemTextJson;
using SplitwiseClient.Model.ApiResponse;
using SplitwiseClient.Model.Expenses;
using SplitwiseClient.Model.Friends;
using SplitwiseClient.Model.Users;

namespace SplitwiseClient
{
    public class Client
    {
        private const string Url = "https://secure.splitwise.com";
        private const string Api = Url + "/api/v3.0";

        private readonly RestClient _client = new RestClient(Url);

        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        public Client(string consumerKey, string consumerSecret)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _client.UseSystemTextJson();
        }

        /// <summary>
        ///     Get an access token from the OAuth server.
        /// </summary>
        /// <returns>The access token.</returns>
        public Task<AccessToken> GetAuthorizationToken()
        {
            var req = new RestRequest("oauth/token");
            req.AddHeader("Accept", "application/json");
            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            req.AddParameter("client_id", _consumerKey);
            req.AddParameter("client_secret", _consumerSecret);
            req.AddParameter("grant_type", "client_credentials");

            return _client.PostAsync<AccessToken>(req);
        }

        /// <summary>
        ///     Set the default access token to be used on each request. If the token is null, no action is performed.
        /// </summary>
        /// <param name="token">The access token to be set as the default</param>
        public void SetAuthorizationToken(AccessToken token)
        {
            if (token != null)
            {
                _client.AddDefaultHeader("Authorization", $"Bearer {token.Token}");
            }
        }

        public async Task ConfigureAccessToken()
        {
            SetAuthorizationToken(await GetAuthorizationToken());
        }

        /// <summary>
        ///     Retrieve info about the user who is currently logged in.
        ///     Sends a GET request to https://secure.splitwise.com/api/v3.0/get_current_user
        /// </summary>
        /// <returns>The current user object.</returns>
        public Task<CurrentUserResponse> GetCurrentUser()
        {
            var req = new RestRequest($"{Api}/get_current_user", DataFormat.Json);
            return _client.GetAsync<CurrentUserResponse>(req);
        }

        /// <summary>
        ///     Retrieve info about another user that the current user is acquainted with (e.g. they are friends, or they both
        ///     belong to the same group).
        ///     Sends a GET request to https://secure.splitwise.com/api/v3.0/get_user/:id
        /// </summary>
        /// <param name="id">The ID of the user to retrieve</param>
        /// <returns>The user object for the ID passed in.</returns>
        public Task<UserResponse> GetUser(int id)
        {
            var req = new RestRequest($"{Api}/get_user/{id}", DataFormat.Json);
            return _client.GetAsync<UserResponse>(req);
        }

        /// <summary>
        ///     Returns a list of the current user's friends.
        ///     Sends a GET request to https://secure.splitwise.com/api/v3.0/get_friends
        /// </summary>
        /// <returns></returns>
        public Task<FriendsResponse> GetFriends()
        {
            var req = new RestRequest($"{Api}/get_friends", DataFormat.Json);
            return _client.GetAsync<FriendsResponse>(req);
        }

        /// <summary>
        ///     Return expenses involving the current user, in reverse chronological order
        ///     Sends a GET request to https://secure.splitwise.com/api/v3.0/get_expenses
        ///     https://dev.splitwise.com/#expenses
        ///     All parameters are optional. Calling this endpoint with no parameters will return the 20 most recent expenses for
        ///     the user.
        ///     The server sets arbitrary (but large) limits on the number of expenses returned if you set a limit of 0 for a user
        ///     with a lot of expenses
        /// </summary>
        /// <param name="groupId">Return expenses for specific group</param>
        /// <param name="friendId">Return expenses for a specific friend that are not in any group</param>
        /// <param name="datedAfter">ISO 8601 Date time. Return expenses later than this date</param>
        /// <param name="datedBefore">ISO 8601 Date time. Return expenses earlier than this date</param>
        /// <param name="updatedAfter">ISO 8601 Date time. Return expenses updated after this date</param>
        /// <param name="updatedBefore"> 	ISO 8601 Date time. Return expenses updated before this date</param>
        /// <param name="limit">limit query parameter (defaults to 20; set to 0 to fetch all)</param>
        /// <param name="offset">Return expenses starting at limit * offset</param>
        /// <returns></returns>
        public Task<ExpensesResponse> GetExpenses(int? groupId = null, int? friendId = null,
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

            return _client.GetAsync<ExpensesResponse>(req);
        }
    }
}
