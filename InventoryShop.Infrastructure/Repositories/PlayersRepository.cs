using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities.Game;
using InventoryShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryShop.Infrastructure.Repositories;

public sealed class PlayersRepository(InventoryShopDbContext context) : IPlayersRepository
{
   public async Task<PlayerEntity> GetPlayerById(Guid id, CancellationToken ct)
   {
      PlayerEntity? player = await context.Players.FindAsync([id], cancellationToken: ct);
      return player ?? throw new KeyNotFoundException($"Player {id} was not found.");
   }

   public async Task<List<PlayerEntity>> GetAllPlayersAsync(CancellationToken ct) =>
      await context.Players.ToListAsync(cancellationToken: ct);

   public async Task AddPlayerAsync(PlayerEntity player, CancellationToken ct) =>
      await context.Players.AddAsync(player, ct);

   public async Task DeletePlayerAsync(Guid id, CancellationToken ct)
   {
      await context.Players
         .Where(p => p.Id == id)
         .ExecuteDeleteAsync(cancellationToken: ct);
   }
}