using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Domain.Errors;

public static class ItemsErrors
{
   public static Error ItemWithIdNotFoundError(Guid itemId) => 
      new(ErrorCode.DomainError, $"Item with id {itemId} not found");

   public static Error CreationFailed(Guid itemId) =>
      new(ErrorCode.DomainError, $"Failed to create item {itemId}");

   public static Error DeletionFailed(Guid itemId) =>
      new(ErrorCode.DomainError, $"Failed to delete item {itemId}");
}