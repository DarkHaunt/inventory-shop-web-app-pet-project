using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Entities;
using InventoryShop.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Hybrid;

namespace InventoryShop.Infrastructure.Caching;

public sealed class CachedPlayerRepository(IPlayersRepository inner, HybridCache cache) : IPlayersRepository
{
   private const string CacheTag = "players";

   private string GetPlayerCacheKey(Guid playerId) =>
      $"player:{playerId}";
   
   private string GetPlayerCacheKey(string playerNickname) =>
      $"player:{playerNickname}";
   
   private string GetAllPlayersCacheKey() =>
      "players:all";
   
   private async Task InvalidateCache(Guid playerId, CancellationToken ct)
   {
      await cache.RemoveAsync(GetAllPlayersCacheKey(), ct);
      await cache.RemoveAsync(GetPlayerCacheKey(playerId), ct);
      await cache.RemoveByTagAsync(CacheTag, ct);
   }
   
   public async Task<PlayerEntity?> GetPlayerById(Guid id, CancellationToken ct)
   {
      PlayerCacheEntry? entry = await cache.GetOrCreateAsync(
         GetPlayerCacheKey(id),
         async token =>
         {
            PlayerEntity? item = await inner.GetPlayerById(id, token);
            return item is null ? null : PlayerCacheEntry.FromEntity(item);
         },
         tags: [CacheTag],
         cancellationToken: ct);

      return entry?.ToEntity();
   }

   public async Task<PlayerEntity?> GetPlayerByNickname(string nickname, CancellationToken ct)
   {
      PlayerCacheEntry? entry = await cache.GetOrCreateAsync(
         GetPlayerCacheKey(nickname),
         async token =>
         {
            PlayerEntity? item = await inner.GetPlayerByNickname(nickname, token);
            return item is null ? null : PlayerCacheEntry.FromEntity(item);
         },
         tags: [CacheTag],
         cancellationToken: ct);
      
      return entry?.ToEntity();
   }

   public async Task<List<PlayerEntity>> GetAllPlayersAsync(CancellationToken ct)
   {
      List<PlayerCacheEntry> entries = await cache.GetOrCreateAsync(
         GetAllPlayersCacheKey(),
         async token =>
         {
            var items = await inner.GetAllPlayersAsync(token);
            return items.Select(PlayerCacheEntry.FromEntity).ToList();
         },
         tags: [CacheTag],
         cancellationToken: ct);

      return entries.Select(e => e.ToEntity()).ToList();
   }

   public async Task<bool> IsNicknameTakenAsync(string nickname, CancellationToken ct) =>
      await inner.IsNicknameTakenAsync(nickname, ct);

   public async Task<bool> IsPasswordTakenAsync(string password, CancellationToken ct) =>
      await inner.IsPasswordTakenAsync(password, ct);

   public async Task AddPlayerAsync(PlayerEntity player, CancellationToken ct)
   {
      await inner.AddPlayerAsync(player, ct);
      await InvalidateCache(player.Id, ct);
   }

   public async Task UpdatePlayerWalletAsync(Guid playerId, Wallet wallet, CancellationToken ct)
   {
      await inner.UpdatePlayerWalletAsync(playerId, wallet, ct);
      await InvalidateCache(playerId, ct);
   }

   public async Task UpdatePlayerLevelAsync(Guid playerId, LevelProgress level, CancellationToken ct)
   {
      await inner.UpdatePlayerLevelAsync(playerId, level, ct);
      await InvalidateCache(playerId, ct);
   }

   public async Task UpdatePlayerNicknameAsync(Guid playerId, string nickname, CancellationToken ct)
   {
      await inner.UpdatePlayerNicknameAsync(playerId, nickname, ct);
      await InvalidateCache(playerId, ct);
   }

   public async Task DeletePlayerAsync(Guid id, CancellationToken ct)
   {
      await inner.DeletePlayerAsync(id, ct);
      await InvalidateCache(id, ct);
   }
}