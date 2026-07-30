using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Specifications;
using Microsoft.Extensions.Caching.Hybrid;

namespace InventoryShop.Infrastructure.Caching;

public sealed class CachedShopOrdersRepository(IShopOrdersRepository inner, HybridCache cache) : IShopOrdersRepository
{
   private const string CacheTag = "shop-orders";
   
   private string GetOrderCacheKey(Guid orderId) =>
      $"order:{orderId}";
   
   private string GetAllOrdersCacheKey() =>
      "orders:all";
   
   private async Task InvalidateCache(Guid orderId, CancellationToken ct)
   {
      await cache.RemoveAsync(GetAllOrdersCacheKey(), ct);
      await cache.RemoveAsync(GetOrderCacheKey(orderId), ct);
      await cache.RemoveByTagAsync(CacheTag, ct);
   }
   
   public async Task<ShopOrderEntity?> GetOrderByIdAsync(Guid id, CancellationToken ct)
   {
      ShopOrderCacheEntry? entry = await cache.GetOrCreateAsync(
         GetOrderCacheKey(id),
         async token =>
         {
            ShopOrderEntity? item = await inner.GetOrderByIdAsync(id, token);
            return item is null ? null : ShopOrderCacheEntry.FromEntity(item);
         },
         tags: [CacheTag],
         cancellationToken: ct);

      return entry?.ToEntity();
   }

   public async Task<List<ShopOrderEntity>> GetAllOrdersAsync(CancellationToken ct)
   {
      List<ShopOrderCacheEntry> entries = await cache.GetOrCreateAsync(
         GetAllOrdersCacheKey(),
         async token =>
         {
            var items = await inner.GetAllOrdersAsync(token);
            return items.Select(ShopOrderCacheEntry.FromEntity).ToList();
         },
         tags: [CacheTag],
         cancellationToken: ct);

      return entries.Select(e => e.ToEntity()).ToList();
   }

   public async Task<List<ShopOrderEntity>> GetOrdersSpecifiedAsync(Specification<ShopOrderEntity> specification, CancellationToken ct) =>
      await inner.GetOrdersSpecifiedAsync(specification, ct);

   public async Task AddOrderAsync(ShopOrderEntity order, CancellationToken ct)
   {
      await inner.AddOrderAsync(order, ct);
      await InvalidateCache(order.Id, ct);
   }

   public async Task DeleteOrderAsync(Guid id, CancellationToken ct)
   {
      await inner.DeleteOrderAsync(id, ct);
      await InvalidateCache(id, ct);
   }
}