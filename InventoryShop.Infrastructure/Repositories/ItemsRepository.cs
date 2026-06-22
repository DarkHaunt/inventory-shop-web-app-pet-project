using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.Enums;
using InventoryShop.Domain.ValueObjects;
using InventoryShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryShop.Infrastructure.Repositories;

public sealed class ItemsRepository(InventoryShopDbContext context) : IItemsRepository
{
   public async Task<ItemEntity> GetItemByIdAsync(Guid id, CancellationToken ct)
   {
      ItemEntity? item = await context.Items.FindAsync([id], cancellationToken: ct);
      return item ?? throw new KeyNotFoundException($"Item {id} was not found.");
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

   public async Task<List<ItemEntity>> GetAllItemsEquippedByAsync(Guid ownerId, CancellationToken ct)
   {
      return await context.Items
         .Where(i => i.OwnerId == ownerId && i.IsEquipped)
         .ToListAsync(cancellationToken: ct);
   }

   public async Task<List<ItemEntity>> GetAllItemsCreatedByAsync(Guid creatorId, CancellationToken ct)
   {
      return await context.Items
         .Where(i => i.CreatorId == creatorId)
         .ToListAsync(cancellationToken: ct);
   }
}