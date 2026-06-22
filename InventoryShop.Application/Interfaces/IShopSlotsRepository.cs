using InventoryShop.Domain.Entities.Shop;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Application.Interfaces;

public interface IShopSlotsRepository
{
   Task<ShopSlotEntity> GetSlotById(Guid id, CancellationToken ct);
   Task<List<ShopSlotEntity>> GetAllSlotsAsync(CancellationToken ct);
   Task<List<ShopSlotEntity>> GetAllSlotsCreatedByAsync(Guid creatorId, CancellationToken ct);
   
   Task AddSlotAsync(ShopSlotEntity slot, CancellationToken ct);
   Task DeleteSlotAsync(Guid id, CancellationToken ct);
}