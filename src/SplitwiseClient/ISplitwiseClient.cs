using System;
using System.Threading.Tasks;
using SplitwiseClient.Model.Expenses;
using SplitwiseClient.Model.Friends;
using SplitwiseClient.Model.Users;

namespace SplitwiseClient
{
    /// <summary>
    /// Interface for interacting with Splitwise.
    /// </summary>
    public interface ISplitwiseClient
    {
        /// <summary>
        ///     Setup client with OAuth access token.
        /// </summary>
        /// <param name="consumerKey">The Splitwise application consumer key.</param>
        /// <param name="consumerSecret">The Splitwise application consumer secret.</param>
        /// <returns>A Task on completion.</returns>
        public Task ConfigureAccessToken(string consumerKey, string consumerSecret);

        /// <summary>
        ///     Retrieve info about the user who is currently logged in.
        ///     Sends a GET request to https://secure.splitwise.com/api/v3.0/get_current_user
        /// </summary>
        /// <returns>The current user as a <see cref="CurrentUserResponse"/> object.</returns>
        public Task<CurrentUserResponse> GetCurrentUser();

        /// <summary>
        ///     Returns a list of the current user's friends.
        ///     Sends a GET request to https://secure.splitwise.com/api/v3.0/get_friends
        /// </summary>
        /// <returns>The list of the current user's friends as a <see cref="FriendResponse"/> object.</returns>
        public Task<FriendsResponse> GetFriends();

        /// <summary>
        ///     Return expenses involving the current user, in reverse chronological order
        ///     Sends a GET request to https://secure.splitwise.com/api/v3.0/get_expenses
        ///     https://dev.splitwise.com/#expenses
        ///     All parameters are optional. Calling this endpoint with no parameters will return the 20 most recent expenses for the user.
        ///     The server sets arbitrary (but large) limits on the number of expenses returned if you set a limit of 0 for a user
        ///     with a lot of expenses
        /// </summary>
        /// <param name="groupId">Return expenses for specific group</param>
        /// <param name="friendId">Return expenses for a specific friend that are not in any group</param>
        /// <param name="datedAfter">ISO 8601 Date time. Return expenses later than this date</param>
        /// <param name="datedBefore">ISO 8601 Date time. Return expenses earlier than this date</param>
        /// <param name="updatedAfter">ISO 8601 Date time. Return expenses updated after this date</param>
        /// <param name="updatedBefore">ISO 8601 Date time. Return expenses updated before this date</param>
        /// <param name="limit">limit query parameter (defaults to 20; set to 0 to fetch all)</param>
        /// <param name="offset">Return expenses starting at limit * offset</param>
        /// <returns>The current user's expenses as a <see cref="ExpensesResponse"/> object.</returns>
        public Task<ExpensesResponse> GetExpenses(int? groupId = null, int? friendId = null,
            DateTimeOffset? datedAfter = null, DateTimeOffset? datedBefore = null, DateTimeOffset? updatedAfter = null,
            DateTimeOffset? updatedBefore = null, int limit = 20, int offset = 0);
    }
}
