using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.ValueObjects;
using InventoryShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryShop.Infrastructure.Repositories;

public sealed class PlayersRepository(InventoryShopDbContext context) : IPlayersRepository
{
   public async Task<PlayerEntity?> GetPlayerById(Guid id, CancellationToken ct) =>
      await context.Players.FindAsync([id], cancellationToken: ct);
   
   public async Task<PlayerEntity?> GetPlayerByNickname(string nickname, CancellationToken ct) =>
      await context.Players.FirstOrDefaultAsync(p => p.Nickname == nickname, cancellationToken: ct);

   public async Task<List<PlayerEntity>> GetAllPlayersAsync(CancellationToken ct) =>
      await context.Players.ToListAsync(cancellationToken: ct);

   public async Task AddPlayerAsync(PlayerEntity player, CancellationToken ct) =>
      await context.Players.AddAsync(player, ct);

   public async Task<bool> IsNicknameTakenAsync(string nickname, CancellationToken ct) =>
      await context.Players.AnyAsync(p => p.Nickname == nickname, cancellationToken: ct);
   
   public async Task<bool> IsPasswordTakenAsync(string password, CancellationToken ct) =>
      await context.Players.AnyAsync(p => p.PasswordHashed == password, cancellationToken: ct);

   public async Task UpdatePlayerNicknameAsync(Guid playerId, string nickname, CancellationToken ct)
   {
      await context.Players
         .Where(p => p.Id == playerId)
         .ExecuteUpdateAsync(b => b.SetProperty(p => p.Nickname, nickname), ct);
   }

   public async Task UpdatePlayerLevelAsync(Guid playerId, LevelProgress level, CancellationToken ct)
   {
      await context.Players
         .Where(p => p.Id == playerId)
         .ExecuteUpdateAsync(b => b.SetProperty(p => p.LevelProgress, level), ct);
   }

   public async Task UpdatePlayerWalletAsync(Guid playerId, Wallet wallet, CancellationToken ct)
   {
      await context.Players
         .Where(p => p.Id == playerId)
         .ExecuteUpdateAsync(b => b.SetProperty(p => p.Wallet, wallet), ct);
   }

   public async Task DeletePlayerAsync(Guid id, CancellationToken ct)
   {
      await context.Players
         .Where(p => p.Id == id)
         .ExecuteDeleteAsync(cancellationToken: ct);
   }
}