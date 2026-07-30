using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Specifications;
using InventoryShop.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Hybrid;

public sealed class CachedItemsRepository(IItemsRepository inner, HybridCache cache) : IItemsRepository
{
   private const string CacheTag = "items";

   private string GetItemCacheKey(Guid itemId) =>
      $"item:{itemId}";
   
   private string GetAllItemsCacheKey() =>
      "items:all";
   
   private async Task InvalidateCache(Guid itemId, CancellationToken ct)
   {
      await cache.RemoveAsync(GetAllItemsCacheKey(), ct);
      await cache.RemoveAsync(GetItemCacheKey(itemId), ct);
      await cache.RemoveByTagAsync(CacheTag, ct);
   }
   
   public async Task<ItemEntity?> GetItemByIdAsync(Guid id, CancellationToken ct)
   {
      ItemCacheEntry? entry = await cache.GetOrCreateAsync(
         GetItemCacheKey(id),
         async token =>
         {
            ItemEntity? item = await inner.GetItemByIdAsync(id, token);
            return item is null ? null : ItemCacheEntry.FromEntity(item);
         },
         tags: [CacheTag],
         cancellationToken: ct);

      return entry?.ToEntity();
   }
   
   public async Task<List<ItemEntity>> GetAllItemsAsync(CancellationToken ct)
   {
      List<ItemCacheEntry> entries = await cache.GetOrCreateAsync(
         GetAllItemsCacheKey(),
         async token =>
         {
            var items = await inner.GetAllItemsAsync(token);
            return items.Select(ItemCacheEntry.FromEntity).ToList();
         },
         tags: [CacheTag],
         cancellationToken: ct);

      return entries.Select(e => e.ToEntity()).ToList();
   }

   public async Task<List<ItemEntity>> GetItemsSpecifiedAsync(Specification<ItemEntity> specification, CancellationToken ct) =>
      await inner.GetItemsSpecifiedAsync(specification, ct);

   public async Task<bool> IsItemOwnedByPlayerAsync(Guid itemId, Guid? ownerId, CancellationToken ct) =>
      await inner.IsItemOwnedByPlayerAsync(itemId, ownerId, ct);

   public async Task AddItemAsync(ItemEntity item, CancellationToken ct)
   {
      await inner.AddItemAsync(item, ct);
      await InvalidateCache(item.Id, ct);
   }
   
   public async Task DeleteItemAsync(Guid id, CancellationToken ct)
   {
      await inner.DeleteItemAsync(id, ct);
      await InvalidateCache(id, ct);
   }

   public async Task UpdateItemEquipStatus(Guid itemId, bool isEquipped, CancellationToken ct)
   {
      await inner.UpdateItemEquipStatus(itemId, isEquipped, ct);
      await InvalidateCache(itemId, ct);
   }

   public async Task UpdateItemSaleStatus(Guid itemId, bool isOnSale, CancellationToken ct)
   {
      await inner.UpdateItemSaleStatus(itemId, isOnSale, ct);
      await InvalidateCache(itemId, ct);
   }

   public async Task UpdateItemOwnership(Guid itemId, Guid? ownerId, CancellationToken ct)
   {
      await inner.UpdateItemOwnership(itemId, ownerId, ct);
      await InvalidateCache(itemId, ct);
   }
}