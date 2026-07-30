using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Specifications;
using InventoryShop.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Hybrid;

namespace InventoryShop.Infrastructure.Caching;

public sealed class CachedShopSlotsRepository(IShopSlotsRepository inner, HybridCache cache) : IShopSlotsRepository
{
   private const string CacheTag = "shop-slots";
   
   private string GetSlotCacheKey(Guid slotId) =>
      $"slot:{slotId}";
   
   private string GetAllSlotsCacheKey() =>
      "slots:all";
   
   private async Task InvalidateCache(Guid slotId, CancellationToken ct)
   {
      await cache.RemoveAsync(GetAllSlotsCacheKey(), ct);
      await cache.RemoveAsync(GetSlotCacheKey(slotId), ct);
      await cache.RemoveByTagAsync(CacheTag, ct);
   }
   
   public async Task<ShopSlotEntity?> GetSlotById(Guid id, CancellationToken ct)
   {
      ShopSlotCacheEntry? entry = await cache.GetOrCreateAsync(
         GetSlotCacheKey(id),
         async token =>
         {
            ShopSlotEntity? item = await inner.GetSlotById(id, token);
            return item is null ? null : ShopSlotCacheEntry.FromEntity(item);
         },
         tags: [CacheTag],
         cancellationToken: ct);

      return entry?.ToEntity();
   }

   public async Task<List<ShopSlotEntity>> GetAllSlotsAsync(CancellationToken ct)
   {
      List<ShopSlotCacheEntry> entries = await cache.GetOrCreateAsync(
         GetAllSlotsCacheKey(),
         async token =>
         {
            var items = await inner.GetAllSlotsAsync(token);
            return items.Select(ShopSlotCacheEntry.FromEntity).ToList();
         },
         tags: [CacheTag],
         cancellationToken: ct);

      return entries.Select(e => e.ToEntity()).ToList();
   }

   public async Task<List<ShopSlotEntity>> GetSlotsSpecifiedAsync(Specification<ShopSlotEntity> specification, CancellationToken ct) =>
      await inner.GetSlotsSpecifiedAsync(specification, ct);

   public async Task<bool> IsSlotOwnedByPlayerAsync(Guid? slotOwnerId, Guid slotId, CancellationToken ct) =>
      await inner.IsSlotOwnedByPlayerAsync(slotOwnerId, slotId, ct);

   public async Task AddSlotAsync(ShopSlotEntity slot, CancellationToken ct)
   {
      await inner.AddSlotAsync(slot, ct);
      await InvalidateCache(slot.Id, ct);
   }

   public async Task UpdateSlotPriceAsync(Guid slotId, Wallet newPrice, CancellationToken ct)
   {
      await inner.UpdateSlotPriceAsync(slotId, newPrice, ct);
      await InvalidateCache(slotId, ct);
   }

   public async Task UpdateSlotRequiredLevelAsync(Guid slotId, LevelProgress newLevel, CancellationToken ct)
   {
      await inner.UpdateSlotRequiredLevelAsync(slotId, newLevel, ct);
      await InvalidateCache(slotId, ct);
   }

   public async Task DeleteSlotAsync(Guid id, CancellationToken ct)
   {
      await inner.DeleteSlotAsync(id, ct);
      await InvalidateCache(id, ct);
   }
}