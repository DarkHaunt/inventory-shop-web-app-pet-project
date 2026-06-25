using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities.Shop;
using InventoryShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryShop.Infrastructure.Repositories;

public sealed class ShopSlotsRepository(InventoryShopDbContext context) : IShopSlotsRepository
{
   public async Task<ShopSlotEntity?> GetSlotById(Guid id, CancellationToken ct) =>
      await context.ShopSlots.FindAsync([id], cancellationToken: ct);

   public async Task<List<ShopSlotEntity>> GetAllSlotsAsync(CancellationToken ct) =>
      await context.ShopSlots.ToListAsync(cancellationToken: ct);

   public async Task<List<ShopSlotEntity>> GetAllSlotsCreatedByAsync(Guid creatorId, CancellationToken ct)
   {
      return await context.ShopSlots
         .Where(s => s.SellerId == creatorId)
         .ToListAsync(cancellationToken: ct);
   }

   public async Task AddSlotAsync(ShopSlotEntity slot, CancellationToken ct) =>
      await context.ShopSlots.AddAsync(slot, ct);

   public async Task DeleteSlotAsync(Guid id, CancellationToken ct)
   {
      await context.ShopSlots
         .Where(s => s.Id == id)
         .ExecuteDeleteAsync(cancellationToken: ct);
   }
}