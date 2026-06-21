namespace InventoryShop.Domain.Shared.Errors;

public static class GenericErrors
{
   public static Error TransactionError(string? message = null) => 
      new(ErrorCode.TransactionError, message is null ? "Transaction error occurred" : $"Transaction error occurred : {message}");

   public static Error OperationCanceledError() => 
      new(ErrorCode.OperationCanceledError, "Operation was canceled");
   
   public static Error DBError(string? message = null) => 
      new(ErrorCode.DBError, message is null ? "DB error occurred" : $"DB error occurred : {message}");
}