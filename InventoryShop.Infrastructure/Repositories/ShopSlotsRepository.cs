using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Specifications;
using InventoryShop.Domain.ValueObjects;
using InventoryShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryShop.Infrastructure.Repositories;

public sealed class ShopSlotsRepository(InventoryShopDbContext context) : IShopSlotsRepository
{
   public async Task<ShopSlotEntity?> GetSlotById(Guid id, CancellationToken ct) =>
      await context.ShopSlots.FindAsync([id], cancellationToken: ct);

   public async Task<List<ShopSlotEntity>> GetAllSlotsAsync(CancellationToken ct) =>
      await context.ShopSlots.ToListAsync(cancellationToken: ct);

   public async Task<List<ShopSlotEntity>> GetSlotsSpecifiedAsync(Specification<ShopSlotEntity> specification, CancellationToken ct)
   {
      return await context.ShopSlots
         .Where(specification.ToExpression())
         .ToListAsync(cancellationToken: ct);
   }
   
   public async Task<bool> IsSlotOwnedByPlayerAsync(Guid? slotOwnerId, Guid slotId, CancellationToken ct) =>
      await context.ShopSlots.AnyAsync(s => s.Id == slotId && s.SellerId == slotOwnerId, cancellationToken: ct);

   public async Task AddSlotAsync(ShopSlotEntity slot, CancellationToken ct) =>
      await context.ShopSlots.AddAsync(slot, ct);

   public async Task UpdateSlotPriceAsync(Guid slotId, Wallet newPrice, CancellationToken ct)
   {
      await context.ShopSlots
         .Where(s => s.Id == slotId)
         .ExecuteUpdateAsync(b => b.SetProperty(s => s.Price, newPrice), ct);
   }

   public async Task UpdateSlotRequiredLevelAsync(Guid slotId, LevelProgress newLevel, CancellationToken ct)
   {
      await context.ShopSlots
         .Where(s => s.Id == slotId)
         .ExecuteUpdateAsync(b => b.SetProperty(s => s.RequiredLevel, newLevel), ct);
   }

   public async Task DeleteSlotAsync(Guid id, CancellationToken ct)
   {
      await context.ShopSlots
         .Where(s => s.Id == id)
         .ExecuteDeleteAsync(cancellationToken: ct);
   }
}