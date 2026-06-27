using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Domain.Errors;

public static class PlayerErrors
{
   public static Error PlayerWithIdNotFoundError(Guid playerId) => 
      new(ErrorCode.DomainError, $"Player with id {playerId} not found");

   public static Error CreationFailed(string nickname) =>
      new (ErrorCode.DomainError, $"Failed to create player {nickname}");

   public static Error DeletionFailed(Guid playerId) =>
      new (ErrorCode.DomainError, $"Failed to delete player {playerId}");
}