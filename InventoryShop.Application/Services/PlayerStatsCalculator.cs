using InventoryShop.Application.Interfaces;
using InventoryShop.Domain.Services;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Application.Services;

public class PlayerStatsCalculator(IItemsRepository itemsRepository, ItemsStatsCalculator statsCalculator)
{
   public async Task<Stats> CalculateStatsOf(Guid playerId, CancellationToken ct)
   {
      var itemsOwnedByPlayer = await itemsRepository.GetAllItemsEquippedByAsync(playerId, ct);
      return statsCalculator.Calculate(itemsOwnedByPlayer);
   }
}