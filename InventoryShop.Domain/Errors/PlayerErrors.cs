using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Domain.Errors;

public static class PlayerErrors
{
   public static Error PlayerWithIdNotFoundError(Guid playerId) => 
      new(ErrorCode.DomainError, $"Player with id {playerId} not found");
}