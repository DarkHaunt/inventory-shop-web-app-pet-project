using InventoryShop.Domain.Entities;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Application.Interfaces;

public interface IPlayersRepository
{
   Task<PlayerEntity?> GetPlayerById(Guid id, CancellationToken ct);
   Task<List<PlayerEntity>> GetAllPlayersAsync(CancellationToken ct);
   Task<bool> IsNicknameAlreadyExistsAsync(string nickname, CancellationToken ct);
   
   Task AddPlayerAsync(PlayerEntity player, CancellationToken ct);
   Task UpdatePlayerWalletAsync(Guid playerId, Wallet wallet, CancellationToken ct);
   Task UpdatePlayerLevelAsync(Guid playerId, LevelProgress level, CancellationToken ct);
   Task UpdatePlayerNicknameAsync(Guid playerId, string nickname, CancellationToken ct);
   Task DeletePlayerAsync(Guid id, CancellationToken ct);
}