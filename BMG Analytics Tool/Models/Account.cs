using BMG_Analytics_Tool.Models;

public class Account
{
    public string AccountId { get; }
    public List<Transaction> Transactions { get; } = new List<Transaction>();

    public Account(string accountId)
    {
        AccountId = accountId;
    }

    public Transaction AddTransaction(DateTime date, string type, decimal amount)
    {
        if (type != "D" && type != "W")
            throw new Exception("Invalid transaction type.");

        if (amount <= 0)
            throw new Exception("Amount must be greater than zero.");

        if (type == "W" && GetBalance() < amount)
            throw new Exception("Insufficient balance.");

        var transactionId = $"{date:yyyyMMdd}-{Transactions.Count + 1:D2}";
        var transaction = new Transaction(date, transactionId, type, amount);
        Transactions.Add(transaction);
        return transaction;
    }

    public decimal GetBalance()
    {
        return Transactions.Sum(t => t.Type == "D" ? t.Amount : -t.Amount);
    }
}

