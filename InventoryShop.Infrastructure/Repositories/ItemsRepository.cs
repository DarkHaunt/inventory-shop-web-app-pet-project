using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities.Game;
using InventoryShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryShop.Infrastructure.Repositories;

public sealed class ItemsRepository(InventoryShopDbContext context) : IItemsRepository
{
   public async Task<ItemEntity?> GetItemByIdAsync(Guid id, CancellationToken ct)
   {
      return await context.Items
         .Include(i => i.Owner)
         .Include(i => i.Creator)
         .AsNoTracking()
         .SingleOrDefaultAsync(i => i.Id == id, ct);
   }

   public async Task<List<ItemEntity>> GetAllItemsAsync(CancellationToken ct) =>
      await context.Items.ToListAsync(cancellationToken: ct);

   public async Task AddItemAsync(ItemEntity item, CancellationToken ct) =>
      await context.Items.AddAsync(item, ct);

   public async Task DeleteItemAsync(Guid id, CancellationToken ct)
   {
      await context.Items
         .Where(i => i.Id == id)
         .ExecuteDeleteAsync(cancellationToken: ct);
   }

   public async Task<List<ItemEntity>> GetAllItemsOwnedByAsync(Guid ownerId, CancellationToken ct)
   {
      return await context.Items
         .Where(i => i.OwnerId == ownerId)
         .Include(i => i.Owner)
         .Include(i => i.Creator)
         .AsNoTracking()
         .ToListAsync(cancellationToken: ct);
   }
   
   public async Task<List<ItemEntity>> GetAllItemsEquippedByAsync(Guid ownerId, CancellationToken ct)
   {
      return await context.Items
         .Where(i => i.OwnerId == ownerId && i.IsEquipped)
         .Include(i => i.Owner)
         .Include(i => i.Creator)
         .AsNoTracking()
         .ToListAsync(cancellationToken: ct);
   }

   public async Task<List<ItemEntity>> GetAllItemsCreatedByAsync(Guid creatorId, CancellationToken ct)
   {
      return await context.Items
         .Where(i => i.CreatorId == creatorId)
         .Include(i => i.Owner)
         .Include(i => i.Creator)
         .AsNoTracking()
         .ToListAsync(cancellationToken: ct);
   }
}