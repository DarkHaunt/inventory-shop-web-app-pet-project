using InventoryShop.Domain.Shared.Errors;

namespace InventoryShop.Domain.Errors;

public static class ItemsErrors
{
   public static Error ItemWithIdNotFoundError(Guid itemId) => 
      new(ErrorCode.DomainError, $"Item with id {itemId} not found");

   public static Error PlayerTriesEquipNotOwnedItem(Guid playerId, Guid itemId) =>
      new(ErrorCode.DomainError, $"Player with id {playerId} does not own item with id {itemId} and cannot equip it");

   public static Error PlayerTriesEquipOnSaleItem(Guid playerId, Guid itemId) =>
      new(ErrorCode.DomainError, $"Player with id {playerId} tries to equip item with id {itemId} that is currently on sale");

   public static Error PlayerDoesNotOwnItem(Guid itemId, Guid? ownerId)
   {
      string player = ownerId.HasValue ? $"Player {ownerId.Value}" : "System";
      return new Error(ErrorCode.DomainError, $"{player} does not own item {itemId}");
   }
}