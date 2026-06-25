using InventoryShop.Domain.Entities.Game;

namespace InventoryShop.Application.Interfaces;

public interface IItemsRepository
{
   Task<ItemEntity?> GetItemByIdAsync(Guid id, CancellationToken ct);
   Task<List<ItemEntity>> GetAllItemsAsync(CancellationToken ct);

   Task AddItemAsync(ItemEntity item, CancellationToken ct);
   Task DeleteItemAsync(Guid id, CancellationToken ct);

   Task<List<ItemEntity>> GetAllItemsOwnedByAsync(Guid ownerId, CancellationToken ct);
   Task<List<ItemEntity>> GetAllItemsEquippedByAsync(Guid ownerId, CancellationToken ct);
   Task<List<ItemEntity>> GetAllItemsCreatedByAsync(Guid creatorId, CancellationToken ct);
}