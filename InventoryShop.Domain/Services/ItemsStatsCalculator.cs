using InventoryShop.Domain.ValueObjects;

namespace InventoryShop.Domain.Services;

public sealed class ItemsStatsCalculator
{
   public Stats Calculate(IEnumerable<Stats> stats) =>
      stats.Aggregate(Stats.CreateInitial(), (total, nextStat) => total.Add(nextStat));
}