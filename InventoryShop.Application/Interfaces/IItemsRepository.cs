using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Specifications;

namespace InventoryShop.Application.Interfaces;

public interface IItemsRepository
{
   Task<ItemEntity?> GetItemByIdAsync(Guid id, CancellationToken ct);
   Task<List<ItemEntity>> GetAllItemsAsync(CancellationToken ct);
   Task<List<ItemEntity>> GetItemsSpecifiedAsync(Specification<ItemEntity> specification, CancellationToken ct);

   Task AddItemAsync(ItemEntity item, CancellationToken ct);
   Task DeleteItemAsync(Guid id, CancellationToken ct);
   Task UpdateItemEquipStatus(Guid itemId, bool isEquipped, CancellationToken ct);
   Task UpdateItemSaleStatus(Guid itemId, bool isOnSale, CancellationToken ct);
   Task UpdateItemOwnership(Guid itemId, Guid? ownerId, CancellationToken ct);
}