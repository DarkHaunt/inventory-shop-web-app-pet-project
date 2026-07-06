using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Domain.Errors;

public static class ShopSlotsErrors
{
   public static Error ShopSlotWithIdNotFoundError(Guid id) =>
      new(ErrorCode.DomainError, $"Shop slot with id {id} not found");

   public static Error CreationFailed(Guid id) =>
      new(ErrorCode.DomainError, $"Failed to create shop slot {id}");

   public static Error DeletionFailed(Guid id) =>
      new(ErrorCode.DomainError, $"Failed to delete shop slot {id}");

   public static Error PlayerTriesSellNotOwnedItem(Guid playerId, Guid itemId) =>
      new(ErrorCode.DomainError, $"Player {playerId} tries to sell item {itemId} that he's not own");

   public static Error SystemTriesSellNotOwnedItem(Guid itemId) =>
      new(ErrorCode.DomainError, $"System tries to sell item {itemId} that it's not own");

   public static Error NoPriceOrLevelRequiredProvided() =>
      new(ErrorCode.DomainError, $"No price or level required provided");
}
