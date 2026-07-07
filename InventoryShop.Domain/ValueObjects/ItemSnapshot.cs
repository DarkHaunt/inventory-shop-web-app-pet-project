using InventoryShop.Domain.Enums;

namespace InventoryShop.Domain.ValueObjects;

public sealed record ItemSnapshot(
   Guid Id,
   ItemType Type,
   string? Description,
   Stats StatsModifiers,
   Guid? CreatorId
);