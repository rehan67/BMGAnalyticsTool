using System.Globalization;
public class Program
{
    private static BankingService _bankingService = new BankingService();

    public static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("Welcome to AwesomeGIC Bank! What would you like to do?");
            Console.WriteLine("[T] Input transactions");
            Console.WriteLine("[I] Define interest rules");
            Console.WriteLine("[P] Print statement");
            Console.WriteLine("[Q] Quit");
            Console.Write("> ");
            var input = Console.ReadLine()?.ToUpper();

            switch (input)
            {
                case "T":
                    InputTransaction();
                    break;
                case "I":
                    DefineInterestRule();
                    break;
                case "P":
                    PrintStatement();
                    break;
                case "Q":
                    Console.WriteLine("Thank you for banking with AwesomeGIC Bank.");
                    Console.WriteLine("Have a nice day!");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private static void InputTransaction()
    {
        Console.WriteLine("Please enter transaction details in <Date> <Account> <Type> <Amount> format");
        Console.WriteLine("(or enter blank to go back to main menu):");
        Console.Write("> ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return;

        var parts = input.Split(' ');
        if (parts.Length != 4)
        {
            Console.WriteLine("Invalid input format. Please try again.");
            return;
        }

        try
        {
            var date = DateTime.ParseExact(parts[0], "yyyyMMdd", CultureInfo.InvariantCulture);
            var accountId = parts[1];
            var type = parts[2].ToUpper();
            var amount = decimal.Parse(parts[3]);

            var transaction = _bankingService.AddTransaction(date, accountId, type, amount);
            Console.WriteLine($"Transaction added: {transaction}");
            PrintAccountStatement(accountId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void DefineInterestRule()
    {
        Console.WriteLine("Please enter interest rules details in <Date> <RuleId> <Rate in %> format");
        Console.WriteLine("(or enter blank to go back to main menu):");
        Console.Write("> ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return;

        var parts = input.Split(' ');
        if (parts.Length != 3)
        {
            Console.WriteLine("Invalid input format. Please try again.");
            return;
        }

        try
        {
            var date = DateTime.ParseExact(parts[0], "yyyyMMdd", CultureInfo.InvariantCulture);
            var ruleId = parts[1];
            var rate = decimal.Parse(parts[2]);

            _bankingService.AddInterestRule(date, ruleId, rate);
            Console.WriteLine("Interest rule added.");
            PrintInterestRules();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void PrintStatement()
    {
        Console.WriteLine("Please enter account and month to generate the statement <Account> <Year><Month>");
        Console.WriteLine("(or enter blank to go back to main menu):");
        Console.Write("> ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            return;

        var parts = input.Split(' ');
        if (parts.Length != 2)
        {
            Console.WriteLine("Invalid input format. Please try again.");
            return;
        }

        try
        {
            var accountId = parts[0];
            var yearMonth = parts[1];
            var year = int.Parse(yearMonth.Substring(0, 4));
            var month = int.Parse(yearMonth.Substring(4, 2));

            var statement = _bankingService.GenerateStatement(accountId, year, month);
            Console.WriteLine(statement);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void PrintAccountStatement(string accountId)
    {
        var transactions = _bankingService.GetTransactionsForAccount(accountId);
        Console.WriteLine($"Account: {accountId}");
        Console.WriteLine("| Date     | Txn Id      | Type | Amount |");
        foreach (var txn in transactions)
        {
            Console.WriteLine($"| {txn.Date:yyyyMMdd} | {txn.TransactionId} | {txn.Type}    | {txn.Amount,7:F2} |");
        }
    }

    private static void PrintInterestRules()
    {
        var rules = _bankingService.GetInterestRules();
        Console.WriteLine("Interest rules:");
        Console.WriteLine("| Date     | RuleId | Rate (%) |");
        foreach (var rule in rules)
        {
            Console.WriteLine($"| {rule.Date:yyyyMMdd} | {rule.RuleId} | {rule.Rate,8:F2} |");
        }
    }
}








