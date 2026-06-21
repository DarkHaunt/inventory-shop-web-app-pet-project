namespace InventoryShop.Domain.Shared.Errors;

public sealed record Error(ErrorCode Code, string Message);

public enum ErrorCode
{
   Unknown = 0,
   
   OperationCanceledError = 1,
   TransactionError = 2,
   DBError = 3,
}