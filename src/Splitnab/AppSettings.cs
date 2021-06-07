using System;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Splitnab
{
    /// <summary>
    /// Object graph for appsettings.json
    /// </summary>
    public record AppSettings
    {
        public Splitwise Splitwise { get; init; }
        public Ynab Ynab { get; init; }
    }

    public record Splitwise
    {
        public string ConsumerKey { get; init; }
        public string ConsumerSecret { get; init; }
        public string FriendEmail { get; init; }
        public DateTimeOffset TransactionsDatedAfter { get; init; }
    }

    public record Ynab
    {
        public string PersonalAccessToken { get; init; }
        public string BudgetName { get; init; }
        public string SplitwiseAccountName { get; init; }
    }
}
