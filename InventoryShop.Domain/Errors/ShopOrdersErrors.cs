using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Domain.Errors;

public static class ShopOrdersErrors
{
   public static Error OrderWithIdNotFoundError(Guid orderId) => 
      new(ErrorCode.DomainError, $"Order with id {orderId} not found");
   
   public static Error CreationFailed(Guid playerId) =>
      new (ErrorCode.DomainError, $"Failed to create order for player {playerId}");
   
   public static Error DeletionFailed(Guid orderId) =>
      new (ErrorCode.DomainError, $"Failed to delete order {orderId}");
}