using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Application.Interfaces;

public interface IPlayersRepository
{
   Task<PlayerEntity?> GetPlayerById(Guid id, CancellationToken ct);
   Task<List<PlayerEntity>> GetAllPlayersAsync(CancellationToken ct);
   
   Task AddPlayerAsync(PlayerEntity player, CancellationToken ct);
   Task UpdatePlayerAsync(Guid id, CancellationToken ct, string? nickname = null, LevelProgress? level = null, Wallet? wallet = null);
   Task DeletePlayerAsync(Guid id, CancellationToken ct);
}