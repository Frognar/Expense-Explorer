namespace ExpenseExplorer.Application.Receipts.Data;

public static class ReceiptRepositoryFactory
{
    public static IReceiptRepository Create() => new InMemoryRepository();
}