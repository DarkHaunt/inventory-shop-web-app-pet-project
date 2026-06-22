using InventoryShop.Domain.Entities.Game;
using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Domain.Services;

public sealed class ItemsStatsCalculator
{
   public Stats Calculate(IEnumerable<ItemEntity> items) =>
      items.Aggregate(Stats.CreateInitial(), (acc, i) => acc.Add(i.StatsModifiers));
}