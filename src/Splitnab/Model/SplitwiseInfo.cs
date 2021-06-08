using System.Collections.Generic;
using SplitwiseClient.Model.Expenses;
using SplitwiseClient.Model.Friends;
using SplitwiseClient.Model.Users;

namespace Splitnab.Model
{
    public record SplitwiseInfo
    {
        public User CurrentUser { get; init; }
        public FriendModel Friend { get; init; }
        public List<Expense> Expenses { get; init; }
    }
}
