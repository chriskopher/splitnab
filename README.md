# splitnab

`splitnab` is a CLI tool to help facilitate my workflow between [Splitwise](https://www.splitwise.com/)
and [YNAB](https://www.youneedabudget.com/).

It performs an import of transactions from your Splitwise account (for a specified friend) to your YNAB budget.

## Splitwise-YNAB Workflow

I only use this integration for one-to-one Splitwise lending-loaning scenarios. This integration treats your Splitwise
balance as an actual account balance to help categorize expenses, but you need to be careful about treating these items
as reimbursed before they actually are. If you are not mindful, you may be relying on money that has not yet been
returned to you.

Example scenario:

1. I lend Ingrid $15.00 for "Dining Out" (total charge: $30 on credit card)
1. Ingrid lends me $10.00 for "Household Goods"
1. Ingrid e-transfers me $5.00 and settles up on Splitwise

And this is what the transactions will be:

| Account      | Transaction     | Category                 | Inflow | Outflow |
| ------------ | --------------- | ------------------------ | -------| ------- |
| Splitwise    | Dining Out      | Dining Out               | $15.00 |         |
| Credit Card  | Dining Out      | Dining Out               |        | $30.00  |
| Splitwise    | Household Goods | Household Goods          |        | $10.00  |
| Splitwise    | Settle up       | Transfer to Bank Account |        | $5.00   |
| Bank Account | Settle up       | Transfer to Bank Account | $5.00  |         |

When the program is run, transactions in the YNAB Splitwise account are created for every transaction in Splitwise. I
can then categorize each transaction into a budget category which allows actual spending to be tracked. Then, the settle
up transaction from Splitwise can be made as a transfer to your bank account (or wherever).

## Getting Started

### Prerequisites

- [.NET 5.0 SDK](https://dotnet.microsoft.com/download)

### Configuration

You will need to register an application with the Splitwise API and obtain a personal access token from YNAB.

1. Register your application with Splitwise [here](https://secure.splitwise.com/apps)
    - Save the consumer key and consumer secret for later
1. Obtain a personal access token from YNAB [here](https://app.youneedabudget.com/settings/developer) and generate a new
   token
    - Save the personal access token for later

Rename (or create) the `appsettings_template.json` to `appsettings.json` in the same directory as the program and
populate with the following contents:

```json
{
    "Splitwise": {
        "ConsumerKey": "<your consumer key>",
        "ConsumerSecret": "<your consumer secret>",
        "FriendEmail": "<your friend's account email>",
        "TransactionsDatedAfter": "<transactions after this date will be imported, format: yyyy-mm-dd>"
    },
    "YNAB": {
        "PersonalAccessToken": "<your personal access token>",
        "BudgetName": "<the name of the budget you wish to import to>",
        "SplitwiseAccountName": "<your account name you use to track splitwise>"
    }
}
```

## Running the Tool

Run the program in the `Splitnab` project and your transactions from Splitwise will be imported to your YNAB budget.
This can be automated with a cron job or similar.

At this point the YNAB transactions can be categorized and approved.
