using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Splitnab.Model;
using SplitwiseClient;

namespace Splitnab
{
    /// <inheritdoc/>
    public class GetSplitwiseInfoOperation : IGetSplitwiseInfoOperation
    {
        private readonly ILogger<GetSplitwiseInfoOperation> _logger;
        private readonly ISplitwiseClient _splitwiseClient;

        public GetSplitwiseInfoOperation(ILogger<GetSplitwiseInfoOperation> logger, ISplitwiseClient splitwiseClient)
        {
            _logger = logger;
            _splitwiseClient = splitwiseClient;
        }

        public async Task<SplitwiseInfo?> Invoke(AppSettings appSettings)
        {
            await _splitwiseClient.ConfigureAccessToken(
                appSettings.Splitwise.ConsumerKey,
                appSettings.Splitwise.ConsumerSecret);

            var currentUser = await _splitwiseClient.GetCurrentUser();
            if (currentUser.User == null)
            {
                _logger.LogWarning("Unable to fetch the current Splitwise user");

                return null;
            }

            var friends = await _splitwiseClient.GetFriends();
            if (friends.Friends == null)
            {
                _logger.LogWarning("Unable to fetch current user's Splitwise friends list");

                return null;
            }

            var friend = friends.Friends?.FirstOrDefault(x => x.Email == appSettings.Splitwise.FriendEmail);
            if (friend == null)
            {
                _logger.LogWarning("Unable to find the specified Splitwise friend");

                return null;
            }

            // Get desired Splitwise expenses
            var expenses = await _splitwiseClient.GetExpenses(
                friendId: friend.Id,
                datedAfter: appSettings.Splitwise.TransactionsDatedAfter,
                limit: 0);
            if (expenses.Expenses == null)
            {
                _logger.LogWarning("No Splitwise expenses found for the specified user and date range");

                return null;
            }

            return new SplitwiseInfo {CurrentUser = currentUser.User, Friend = friend, Expenses = expenses.Expenses};
        }
    }
}
