public class Transaction
{
    public DateTime Date { get; }
    public string TransactionId { get; }
    public string Type { get; }
    public decimal Amount { get; }

    public Transaction(DateTime date, string transactionId, string type, decimal amount)
    {
        Date = date;
        TransactionId = transactionId;
        Type = type;
        Amount = amount;
    }

    public override string ToString()
    {
        return $"{Date:yyyyMMdd} {TransactionId} {Type} {Amount:F2}";
    }
}

