using InventoryShop.Domain.Enums;

namespace InventoryShop.Web.DTO;

public sealed record ItemDTO(
   Guid Id,
   ItemType Type,
   string? Description,
   StatsDTO StatsModifiers,
   bool IsEquipped,
   bool IsOnSale,
   string? OwnerName,
   string? CreatorName
);

public sealed record ItemSnapshotDTO(
   Guid Id,
   ItemType Type,
   string? Description,
   StatsDTO StatsModifiers,
   Guid? CreatorId
);