using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.Shared.Specifications;
using InventoryShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryShop.Infrastructure.Repositories;

public sealed class ItemsRepository(InventoryShopDbContext context) : IItemsRepository
{
   public async Task<ItemEntity?> GetItemByIdAsync(Guid id, CancellationToken ct)
   {
      return await context.Items
         .AsNoTracking()
         .SingleOrDefaultAsync(i => i.Id == id, ct);
   }

   public async Task<List<ItemEntity>> GetAllItemsAsync(CancellationToken ct) =>
      await context.Items.ToListAsync(cancellationToken: ct);

   public Task<List<ItemEntity>> GetItemsSpecifiedAsync(Specification<ItemEntity> specification, CancellationToken ct)
   {
      return context.Items
         .Where(specification.ToExpression())
         .AsNoTracking()
         .ToListAsync(cancellationToken: ct);
   }

   public async Task AddItemAsync(ItemEntity item, CancellationToken ct) =>
      await context.Items.AddAsync(item, ct);

   public async Task DeleteItemAsync(Guid id, CancellationToken ct)
   {
      await context.Items
         .Where(i => i.Id == id)
         .ExecuteDeleteAsync(cancellationToken: ct);
   }

   public async Task UpdateItemEquipStatus(Guid itemId, bool isEquipped, CancellationToken ct)
   {
      await context.Items
         .Where(i => i.Id == itemId)
         .ExecuteUpdateAsync(b => b.SetProperty(i => i.IsEquipped, isEquipped), ct);
   }
   
   public async Task UpdateItemSaleStatus(Guid itemId, bool isOnSale, CancellationToken ct)
   {
      await context.Items
         .Where(i => i.Id == itemId)
         .ExecuteUpdateAsync(b => b.SetProperty(i => i.IsOnSale, isOnSale), ct);
   }

   public async Task UpdateItemOwnership(Guid itemId, Guid? ownerId, CancellationToken ct)
   {
      await context.Items
         .Where(i => i.Id == itemId)
         .ExecuteUpdateAsync(b => b.SetProperty(i => i.OwnerId, ownerId), ct);
   }
}