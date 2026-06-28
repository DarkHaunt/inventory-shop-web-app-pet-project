using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.ValueObjects;
using InventoryShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryShop.Infrastructure.Repositories;

public sealed class PlayersRepository(InventoryShopDbContext context) : IPlayersRepository
{
   public async Task<PlayerEntity?> GetPlayerById(Guid id, CancellationToken ct) =>
      await context.Players.FindAsync([id], cancellationToken: ct);

   public async Task<List<PlayerEntity>> GetAllPlayersAsync(CancellationToken ct) =>
      await context.Players.ToListAsync(cancellationToken: ct);

   public async Task AddPlayerAsync(PlayerEntity player, CancellationToken ct) =>
      await context.Players.AddAsync(player, ct);
   
   public async Task<bool> IsNicknameAlreadyExistsAsync(string nickname, CancellationToken ct) =>
      await context.Players.AnyAsync(p => p.Nickname == nickname, cancellationToken: ct);

   public async Task UpdatePlayerAsync(Guid id, CancellationToken ct, string? nickname = null, LevelProgress? level = null, Wallet? wallet = null)
   {
      if (nickname is null && level is null && wallet is null) 
         return;
      
      await context.Players.Where(p => p.Id == id)
         .ExecuteUpdateAsync
         (
            b => b
               .SetProperty(p => p.Nickname, p => nickname ?? p.Nickname)
               .SetProperty(p => p.LevelProgress, p => level ?? p.LevelProgress)
               .SetProperty(p => p.Wallet, p => wallet ?? p.Wallet),
            ct
         );
   }

   public async Task DeletePlayerAsync(Guid id, CancellationToken ct)
   {
      await context.Players
         .Where(p => p.Id == id)
         .ExecuteDeleteAsync(cancellationToken: ct);
   }
}