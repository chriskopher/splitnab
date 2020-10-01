using System;
using System.Globalization;

namespace ClientRunner
{
    public class AppSettings
    {
        public AppSettings(dynamic json)
        {
            SwConsumerKey = (string)json.Splitwise.ConsumerKey;
            SwConsumerSecret = (string)json.Splitwise.ConsumerSecret;
            SwFriendEmail = (string)json.Splitwise.FriendEmail;
            SwTransactionsDatedAfter = DateTimeOffset.ParseExact(
                (string)json.Splitwise.TransactionsDatedAfter,
                new[] {"yyyy-MM-dd"},
                CultureInfo.InvariantCulture);
            YnabPersonalAccessToken = (string)json.YNAB.PersonalAccessToken;
            YnabBudgetName = (string)json.YNAB.BudgetName;
            YnabSplitwiseBudgetName = (string)json.YNAB.SplitwiseAccountName;
        }

        public string SwConsumerKey { get; }
        public string SwConsumerSecret { get; }
        public string SwFriendEmail { get; }
        public DateTimeOffset SwTransactionsDatedAfter { get; }
        public string YnabPersonalAccessToken { get; }
        public string YnabBudgetName { get; }
        public string YnabSplitwiseBudgetName { get; }
    }
}
