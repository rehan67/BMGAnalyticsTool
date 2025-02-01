
using BMG_Analytics_Tool.Models;

public class BankingService
{
    private List<Account> _accounts = new List<Account>();
    private List<InterestRule> _interestRules = new List<InterestRule>();

    public Transaction AddTransaction(DateTime date, string accountId, string type, decimal amount)
    {
        var account = _accounts.FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
        {
            account = new Account(accountId);
            _accounts.Add(account);
        }

        var transaction = account.AddTransaction(date, type, amount);
        return transaction;
    }

    public void AddInterestRule(DateTime date, string ruleId, decimal rate)
    {
        var existingRule = _interestRules.FirstOrDefault(r => r.Date == date && r.RuleId == ruleId);
        if (existingRule != null)
        {
            _interestRules.Remove(existingRule);
        }

        _interestRules.Add(new InterestRule(date, ruleId, rate));
        _interestRules = _interestRules.OrderBy(r => r.Date).ToList();
    }

    public List<Transaction> GetTransactionsForAccount(string accountId)
    {
        var account = _accounts.FirstOrDefault(a => a.AccountId == accountId);
        return account?.Transactions ?? new List<Transaction>();
    }

    public List<InterestRule> GetInterestRules()
    {
        return _interestRules;
    }

    public string GenerateStatement(string accountId, int year, int month)
    {
        var account = _accounts.FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
            throw new Exception("Account not found.");

        var transactions = account.Transactions
            .Where(t => t.Date.Year == year && t.Date.Month == month)
            .OrderBy(t => t.Date)
            .ToList();

        var statement = $"Account: {accountId}\n";
        statement += "| Date     | Txn Id      | Type | Amount | Balance |\n";

        decimal balance = 0;
        foreach (var txn in transactions)
        {
            balance += txn.Type == "D" ? txn.Amount : -txn.Amount;
            statement += $"| {txn.Date:yyyyMMdd} | {txn.TransactionId} | {txn.Type}    | {txn.Amount,7:F2} | {balance,7:F2} |\n";
        }

        // Calculate and add interest
        var interest = CalculateInterest(account, year, month);
        if (interest > 0)
        {
            balance += interest;
            statement += $"| {new DateTime(year, month, DateTime.DaysInMonth(year, month)):yyyyMMdd} |             | I    | {interest,7:F2} | {balance,7:F2} |\n";
        }

        return statement;
    }

    private decimal CalculateInterest(Account account, int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        var transactions = account.Transactions
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .OrderBy(t => t.Date)
            .ToList();

        decimal interest = 0;
        decimal balance = 0;
        DateTime currentDate = startDate;

        foreach (var txn in transactions)
        {
            var days = (txn.Date - currentDate).Days;
            if (days > 0)
            {
                var rate = GetApplicableInterestRate(currentDate);
                interest += balance * rate / 100 * days / 365;
            }

            balance += txn.Type == "D" ? txn.Amount : -txn.Amount;
            currentDate = txn.Date;
        }

        // Calculate interest for the remaining days
        var remainingDays = (endDate - currentDate).Days;
        if (remainingDays > 0)
        {
            var rate = GetApplicableInterestRate(currentDate);
            interest += balance * rate / 100 * remainingDays / 365;
        }

        return Math.Round(interest, 2);
    }

    private decimal GetApplicableInterestRate(DateTime date)
    {
        var rule = _interestRules
            .Where(r => r.Date <= date)
            .OrderByDescending(r => r.Date)
            .FirstOrDefault();

        return rule?.Rate ?? 0;
    }
}
