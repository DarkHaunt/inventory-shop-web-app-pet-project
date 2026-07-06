using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Specifications;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Application.Interfaces;

public interface IShopSlotsRepository
{
   Task<ShopSlotEntity?> GetSlotById(Guid id, CancellationToken ct);
   Task<List<ShopSlotEntity>> GetAllSlotsAsync(CancellationToken ct);
   Task<List<ShopSlotEntity>> GetSlotsSpecifiedAsync(Specification<ShopSlotEntity> specification, CancellationToken ct);
   
   Task AddSlotAsync(ShopSlotEntity slot, CancellationToken ct);
   Task UpdateSlotPriceAsync(Guid slotId, Wallet newPrice, CancellationToken ct);
   Task UpdateSlotRequiredLevelAsync(Guid slotId, LevelProgress newLevel, CancellationToken ct);
   Task DeleteSlotAsync(Guid id, CancellationToken ct);
}