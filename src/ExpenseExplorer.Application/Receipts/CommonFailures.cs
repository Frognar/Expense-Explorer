namespace ExpenseExplorer.Application.Receipts;

using FunctionalCore.Failures;

public static class CommonFailures
{
  public static readonly ValidationFailure InvalidReceiptId = Failure.Validation("ReceiptId", "INVALID_RECEIPT_ID");
}
