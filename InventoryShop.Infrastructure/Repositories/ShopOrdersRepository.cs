using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities.Shop;
using InventoryShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryShop.Infrastructure.Repositories;

public sealed class ShopOrdersRepository(InventoryShopDbContext context) : IShopOrdersRepository
{
   public async Task<ShopOrderEntity?> GetOrderByIdAsync(Guid id, CancellationToken ct) =>
      await context.ShopOrders.FindAsync([id], cancellationToken: ct);

   public async Task<List<ShopOrderEntity>> GetAllOrdersAsync(CancellationToken ct) =>
      await context.ShopOrders.ToListAsync(cancellationToken: ct);

   public async Task<List<ShopOrderEntity>> GetAllOrdersCompletedByAsync(Guid buyerId, CancellationToken ct)
   {
      return await context.ShopOrders
         .Where(o => o.BuyerId == buyerId)
         .ToListAsync(cancellationToken: ct);
   }

   public async Task<List<ShopOrderEntity>> GetAllOrdersCreatedByAsync(Guid sellerId, CancellationToken ct)
   {
      return await context.ShopOrders
         .Where(o => o.SellerId == sellerId)
         .ToListAsync(cancellationToken: ct);
   }

   public async Task AddOrderAsync(ShopOrderEntity order, CancellationToken ct) =>
      await context.ShopOrders.AddAsync(order, ct);

   public async Task DeleteOrderAsync(Guid id, CancellationToken ct)
   {
      await context.ShopOrders
         .Where(p => p.Id == id)
         .ExecuteDeleteAsync(cancellationToken: ct);
   }
}